using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using AI_Sales_Agent.Services;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.DashboardManagement.RefreshDashboardInsights;

public class RefreshDashboardInsightsCommandHandler : IRequestHandler<RefreshDashboardInsightsCommand>
{
    private readonly IMongoDbContext _context;
    private readonly IDashboardNotifier _notifier;
    private readonly ILogger<RefreshDashboardInsightsCommandHandler> _logger;

    public RefreshDashboardInsightsCommandHandler(
        IMongoDbContext context,
        IDashboardNotifier notifier,
        ILogger<RefreshDashboardInsightsCommandHandler> logger)
    {
        _context = context;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task Handle(RefreshDashboardInsightsCommand request, CancellationToken cancellationToken)
    {
        var storeId = request.Order.StoreId;
        var orderRevenue = request.Order.TotalPrice?.Amount ?? 0.0;
        var filter = Builders<DashboardInsightDocument>.Filter.Eq(dashboard => dashboard.StoreId, storeId);
        var dashboard = await _context.DashboardInsights
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        var now = DateTime.UtcNow;
        double totalRevenue;
        double growthPercentage;

        if (dashboard is null)
        {
            totalRevenue = orderRevenue;
            growthPercentage = CalculateGrowthPercentage(0.0, totalRevenue);

            var newDashboard = new DashboardInsightDocument
            {
                StoreId = storeId,
                TotalRevenue = totalRevenue,
                GrowthPercentage = growthPercentage,
                CalculatedAt = now,
                UpdatedAt = now
            };

            await _context.DashboardInsights.InsertOneAsync(newDashboard, cancellationToken: cancellationToken);
        }
        else
        {
            totalRevenue = dashboard.TotalRevenue + orderRevenue;
            growthPercentage = CalculateGrowthPercentage(dashboard.OldTotalRevenue, totalRevenue);

            var update = Builders<DashboardInsightDocument>.Update
                .Set(insight => insight.TotalRevenue, totalRevenue)
                .Set(insight => insight.GrowthPercentage, growthPercentage)
                .Set(insight => insight.CalculatedAt, now)
                .Set(insight => insight.UpdatedAt, now);

            await _context.DashboardInsights.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        var growthStatus = growthPercentage switch
        {
            > 0 => "Positive",
            < 0 => "Negative",
            _ => "Neutral"
        };

        try
        {
            await _notifier.NotifyTotalRevenueUpdatedAsync(storeId, totalRevenue);
            await _notifier.NotifyRevenueGrowthUpdatedAsync(
                storeId,
                new { storeId, growthPercentage, status = growthStatus });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to notify dashboard clients for store {StoreId}", storeId);
        }
    }

    private static double CalculateGrowthPercentage(double oldTotalRevenue, double totalRevenue)
    {
        if (oldTotalRevenue == 0)
        {
            return totalRevenue > 0 ? 100.0 : 0.0;
        }

        return Math.Round(((totalRevenue - oldTotalRevenue) / oldTotalRevenue) * 100, 2);
    }
}
