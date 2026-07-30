using AI_Sales_Agent.Data;
using AI_Sales_Agent.Features.Admin.Shared;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminRevenueGrowth;

public class GetAdminRevenueGrowthHandler : IRequestHandler<GetAdminRevenueGrowthQuery, IReadOnlyList<AdminGrowthPointResponse>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMongoDbContext _mongoDbContext;

    public GetAdminRevenueGrowthHandler(ApplicationDbContext dbContext, IMongoDbContext mongoDbContext)
    {
        _dbContext = dbContext;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<IReadOnlyList<AdminGrowthPointResponse>> Handle(
        GetAdminRevenueGrowthQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);
        var trendStart = currentMonthStart.AddMonths(-5);

        var revenueTrend = await GetMonthlyRevenueTrend(trendStart, cancellationToken);
        var conversationTrend = await GetMonthlyConversationTrend(trendStart, cancellationToken);

        return MergeTrends(revenueTrend, conversationTrend);
    }

    private async Task<List<AdminMonthlyRevenuePoint>> GetMonthlyRevenueTrend(
        DateTime trendStart,
        CancellationToken cancellationToken)
    {
        var rawRevenue = await _dbContext.Subscriptions
            .AsNoTracking()
            .Where(subscription =>
                subscription.DeletedAt == null &&
                subscription.Status == "Active" &&
                subscription.CreatedAt >= trendStart &&
                subscription.Plan != null)
            .GroupBy(subscription => new { subscription.CreatedAt.Year, subscription.CreatedAt.Month })
            .Select(group => new
            {
                group.Key.Year,
                group.Key.Month,
                Revenue = group.Sum(subscription => subscription.Plan == null ? 0 : subscription.Plan.PlanPrice)
            })
            .ToListAsync(cancellationToken);

        return Enumerable.Range(0, 6)
            .Select(offset => trendStart.AddMonths(offset))
            .Select(month =>
            {
                var value = rawRevenue.FirstOrDefault(point => point.Year == month.Year && point.Month == month.Month);
                return new AdminMonthlyRevenuePoint(month.Year, month.Month, value?.Revenue ?? 0);
            })
            .ToList();
    }

    private async Task<List<AdminMonthlyConversationPoint>> GetMonthlyConversationTrend(
        DateTime trendStart,
        CancellationToken cancellationToken)
    {
        var rawConversationCounts = await _mongoDbContext.Conversations.Aggregate()
            .Match(new BsonDocument
            {
                ["deleted_at"] = BsonNull.Value,
                ["created_at"] = new BsonDocument("$gte", trendStart)
            })
            .Group(new BsonDocument
            {
                ["_id"] = new BsonDocument
                {
                    ["year"] = new BsonDocument("$year", "$created_at"),
                    ["month"] = new BsonDocument("$month", "$created_at")
                },
                ["count"] = new BsonDocument("$sum", 1)
            })
            .ToListAsync(cancellationToken);

        return Enumerable.Range(0, 6)
            .Select(offset => trendStart.AddMonths(offset))
            .Select(month =>
            {
                var value = rawConversationCounts.FirstOrDefault(point =>
                    point["_id"]["year"].ToInt32() == month.Year &&
                    point["_id"]["month"].ToInt32() == month.Month);

                return new AdminMonthlyConversationPoint(
                    month.Year,
                    month.Month,
                    value?.GetValue("count", 0).ToInt64() ?? 0);
            })
            .ToList();
    }

    private static List<AdminGrowthPointResponse> MergeTrends(
        IReadOnlyCollection<AdminMonthlyRevenuePoint> revenueTrend,
        IReadOnlyCollection<AdminMonthlyConversationPoint> conversationTrend)
    {
        return revenueTrend
            .Select(revenue =>
            {
                var conversations = conversationTrend.FirstOrDefault(point =>
                    point.Year == revenue.Year && point.Month == revenue.Month);

                return new AdminGrowthPointResponse(
                    revenue.Year,
                    revenue.Month,
                    $"{revenue.Year}-{revenue.Month:00}",
                    revenue.Revenue,
                    conversations?.Conversations ?? 0);
            })
            .ToList();
    }
}
