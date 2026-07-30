using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.DashboardManagement.GetTotalRevenue;

public class GetTotalRevenueHandler : IRequestHandler<GetTotalRevenueQuery, double>
{
    private readonly MongoDbContext _context;

    public GetTotalRevenueHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<double> Handle(GetTotalRevenueQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<DashboardInsightDocument>.Filter.Eq(d => d.StoreId, request.StoreId);

        var insights = await _context.DashboardInsights
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);

        // لو المتجر ملوش سجلات سابقة بيرجع 0
        return insights?.TotalRevenue ?? 0.0;
    }
}