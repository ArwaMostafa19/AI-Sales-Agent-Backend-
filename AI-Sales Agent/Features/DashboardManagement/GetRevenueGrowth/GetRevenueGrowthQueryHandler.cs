using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.DashboardManagement.GetRevenueGrowth;

public class GetRevenueGrowthQueryHandler : IRequestHandler<GetRevenueGrowthQuery, RevenueGrowthResponseDto>
{
    private readonly MongoDbContext _context;

    public GetRevenueGrowthQueryHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<RevenueGrowthResponseDto> Handle(GetRevenueGrowthQuery request, CancellationToken cancellationToken)
    {
        // 1. الفلترة بنفس طريقة GetTotalRevenue
        string storeIdStr = request.StoreId.ToString().ToLower();
        var filter = Builders<DashboardInsightDocument>.Filter.Eq(d => d.StoreId, storeIdStr);

        // 2. استخدام _context.DashboardInsights لضمان نفس الـ Collection
        var existingInsight = await _context.DashboardInsights
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        // 3. قراءة القيم
        double currentTotalRevenue = existingInsight?.TotalRevenue ?? 0.0;
        double oldTotalRevenue = existingInsight?.OldTotalRevenue ?? 0.0;

        // 4. حساب نسبة النمو
        double growthPercentage = CalculateGrowthPercentage(oldTotalRevenue, currentTotalRevenue);

        string status = growthPercentage switch
        {
            > 0 => "Positive",
            < 0 => "Negative",
            _ => "Neutral"
        };

        return new RevenueGrowthResponseDto(growthPercentage, status);
    }

    private static double CalculateGrowthPercentage(double oldRevenue, double newRevenue)
    {
        if (oldRevenue == 0 && newRevenue == 0) return 0.0;
        if (oldRevenue == 0) return 100.0;

        double percentage = ((newRevenue - oldRevenue) / oldRevenue) * 100;
        return Math.Round(percentage, 2);
    }
}