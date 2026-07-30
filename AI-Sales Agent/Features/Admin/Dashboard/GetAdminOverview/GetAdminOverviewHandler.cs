using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Features.Admin.Shared;
using AI_Sales_Agent.Infrastructure.Auth;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminOverview;

public class GetAdminOverviewHandler : IRequestHandler<GetAdminOverviewQuery, AdminOverviewResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMongoDbContext _mongoDbContext;

    public GetAdminOverviewHandler(ApplicationDbContext dbContext, IMongoDbContext mongoDbContext)
    {
        _dbContext = dbContext;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<AdminOverviewResponse> Handle(GetAdminOverviewQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var trendStart = currentMonthStart.AddMonths(-5);

        var storeStats = await GetStoreStats(currentMonthStart, previousMonthStart, cancellationToken);
        var userStats = await GetUserStats(cancellationToken);
        var revenueStats = await GetRevenueStats(cancellationToken);
        var conversationStats = await GetConversationStats(currentMonthStart, previousMonthStart, cancellationToken);
        var aiStats = await AdminDashboardQueryHelpers.GetAiStats(_mongoDbContext, cancellationToken);
        var revenueTrend = await GetMonthlyRevenueTrend(trendStart, cancellationToken);
        var conversationTrend = await GetMonthlyConversationTrend(trendStart, cancellationToken);
        var recentStores = await GetRecentStores(8, cancellationToken);
        var recentAuditLogs = await GetRecentAuditLogs(8, cancellationToken);

        return new AdminOverviewResponse(
            new AdminKpiResponse(
                storeStats.TotalStores,
                storeStats.ActiveStores,
                storeStats.ThisMonthGrowthPercent,
                userStats.TotalUsers,
                userStats.TotalSellers,
                userStats.EmailConfirmedUsers,
                conversationStats.TotalConversations,
                conversationStats.ThisMonthGrowthPercent,
                revenueStats.ActiveSubscriptions,
                revenueStats.MonthlyRecurringRevenue,
                aiStats.ConversionRate,
                aiStats.HighIntentMessages),
            storeStats.PlatformDistribution,
            MergeTrends(revenueTrend, conversationTrend),
            aiStats.TopIntents,
            aiStats.SentimentBreakdown,
            recentStores,
            recentAuditLogs);
    }

    private async Task<StoreStats> GetStoreStats(
        DateTime currentMonthStart,
        DateTime previousMonthStart,
        CancellationToken cancellationToken)
    {
        var storesQuery = _dbContext.Stores.AsNoTracking().Where(store => store.DeletedAt == null);
        var totalStores = await storesQuery.CountAsync(cancellationToken);
        var activeStores = await storesQuery.CountAsync(store => store.Status == "Active", cancellationToken);
        var currentMonthStores = await storesQuery.CountAsync(store => store.CreatedAt >= currentMonthStart, cancellationToken);
        var previousMonthStores = await storesQuery.CountAsync(
            store => store.CreatedAt >= previousMonthStart && store.CreatedAt < currentMonthStart,
            cancellationToken);

        var platformCounts = await storesQuery
            .GroupBy(store => string.IsNullOrWhiteSpace(store.Platform) ? "Unknown" : store.Platform)
            .Select(group => new CountBreakdownItem(
                group.Key,
                group.Count(),
                AdminDashboardQueryHelpers.ToPercent(group.Count(), totalStores)))
            .ToListAsync(cancellationToken);

        return new StoreStats(
            totalStores,
            activeStores,
            AdminDashboardQueryHelpers.GrowthPercent(currentMonthStores, previousMonthStores),
            platformCounts);
    }

    private async Task<UserStats> GetUserStats(CancellationToken cancellationToken)
    {
        var totalUsers = await _dbContext.Users.AsNoTracking().CountAsync(cancellationToken);
        var emailConfirmedUsers = await _dbContext.Users
            .AsNoTracking()
            .CountAsync(user => user.EmailConfirmed, cancellationToken);
        var sellerRoleId = await _dbContext.Roles
            .AsNoTracking()
            .Where(role => role.Name == Roles.Seller)
            .Select(role => role.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var totalSellers = sellerRoleId == Guid.Empty
            ? 0
            : await _dbContext.UserRoles
                .AsNoTracking()
                .CountAsync(userRole => userRole.RoleId == sellerRoleId, cancellationToken);

        return new UserStats(totalUsers, totalSellers, emailConfirmedUsers);
    }

    private async Task<RevenueStats> GetRevenueStats(CancellationToken cancellationToken)
    {
        var activeSubscriptionsQuery = _dbContext.Subscriptions
            .AsNoTracking()
            .Where(subscription =>
                subscription.DeletedAt == null &&
                subscription.Status == "Active" &&
                subscription.Plan != null &&
                subscription.Plan.DeletedAt == null);

        var activeSubscriptions = await activeSubscriptionsQuery.CountAsync(cancellationToken);
        var monthlyRecurringRevenue = await activeSubscriptionsQuery
            .SumAsync(subscription => subscription.Plan == null ? 0 : subscription.Plan.PlanPrice, cancellationToken);

        return new RevenueStats(activeSubscriptions, monthlyRecurringRevenue);
    }

    private async Task<ConversationStats> GetConversationStats(
        DateTime currentMonthStart,
        DateTime previousMonthStart,
        CancellationToken cancellationToken)
    {
        var deletedFilter = Builders<ConversationDocument>.Filter.Eq(conversation => conversation.DeletedAt, null);
        var totalConversations = await _mongoDbContext.Conversations.CountDocumentsAsync(
            deletedFilter,
            null,
            cancellationToken);

        var currentMonthFilter = deletedFilter & Builders<ConversationDocument>.Filter.Gte(
            conversation => conversation.CreatedAt,
            currentMonthStart);
        var previousMonthFilter = deletedFilter &
            Builders<ConversationDocument>.Filter.Gte(conversation => conversation.CreatedAt, previousMonthStart) &
            Builders<ConversationDocument>.Filter.Lt(conversation => conversation.CreatedAt, currentMonthStart);

        var currentMonthConversations = await _mongoDbContext.Conversations.CountDocumentsAsync(
            currentMonthFilter,
            null,
            cancellationToken);
        var previousMonthConversations = await _mongoDbContext.Conversations.CountDocumentsAsync(
            previousMonthFilter,
            null,
            cancellationToken);

        return new ConversationStats(
            totalConversations,
            AdminDashboardQueryHelpers.GrowthPercent(currentMonthConversations, previousMonthConversations));
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

    private async Task<List<AdminStoreResponse>> GetRecentStores(int count, CancellationToken cancellationToken)
    {
        return await AdminDashboardQueryHelpers.BuildStoresQuery(_dbContext, null, null, null)
            .OrderByDescending(store => store.CreatedAt)
            .Take(count)
            .SelectAdminStoreResponse()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<AdminAuditLogResponse>> GetRecentAuditLogs(int count, CancellationToken cancellationToken)
    {
        return await _dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(log => log.CreatedAt)
            .Take(count)
            .Select(log => new AdminAuditLogResponse(
                log.Id,
                log.UserId,
                log.Action,
                log.IpAddress,
                log.UserAgent,
                log.Metadata,
                log.CreatedAt))
            .ToListAsync(cancellationToken);
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

    private sealed record StoreStats(
        int TotalStores,
        int ActiveStores,
        decimal ThisMonthGrowthPercent,
        List<CountBreakdownItem> PlatformDistribution);

    private sealed record UserStats(int TotalUsers, int TotalSellers, int EmailConfirmedUsers);

    private sealed record RevenueStats(int ActiveSubscriptions, decimal MonthlyRecurringRevenue);

    private sealed record ConversationStats(long TotalConversations, decimal ThisMonthGrowthPercent);
}
