using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using AI_Sales_Agent.Services;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.DashboardManagement.GetTotalRevenue;

public class GetTotalRevenueHandler : IRequestHandler<GetTotalRevenueQuery, double>
{
    private readonly MongoDbContext _context;
    private readonly IDashboardNotifier _notifier;

    public GetTotalRevenueHandler(MongoDbContext context, IDashboardNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task<double> Handle(GetTotalRevenueQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<DashboardInsightDocument>.Filter.Eq(d => d.StoreId, request.StoreId);

        var insights = await _context.DashboardInsights
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        var totalRevenue = insights?.TotalRevenue ?? 0.0;

        //await _notifier.NotifyTotalRevenueUpdatedAsync(request.StoreId, totalRevenue);

        return totalRevenue;
    }
}