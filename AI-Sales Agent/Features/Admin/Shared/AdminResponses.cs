namespace AI_Sales_Agent.Features.Admin.Shared;

public record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}

public record AdminOverviewResponse(
    AdminKpiResponse Kpis,
    IReadOnlyList<CountBreakdownItem> PlatformDistribution,
    IReadOnlyList<AdminGrowthPointResponse> RevenueConversationTrend,
    IReadOnlyList<CountBreakdownItem> TopIntents,
    IReadOnlyList<CountBreakdownItem> SentimentBreakdown,
    IReadOnlyList<AdminStoreResponse> RecentStores,
    IReadOnlyList<AdminAuditLogResponse> RecentAuditLogs);

public record AdminKpiResponse(
    int TotalStores,
    int ActiveStores,
    decimal StoreGrowthPercent,
    int TotalUsers,
    int TotalSellers,
    int EmailConfirmedUsers,
    long TotalConversations,
    decimal ConversationGrowthPercent,
    int ActiveSubscriptions,
    decimal MonthlyRecurringRevenue,
    decimal AiConversionRate,
    long HighIntentMessages);

public record AdminGrowthPointResponse(
    int Year,
    int Month,
    string Label,
    decimal Revenue,
    long Conversations);

public record CountBreakdownItem(string Label, long Count, decimal Percentage);

public record AdminStoreResponse(
    Guid Id,
    string Name,
    string Platform,
    string ShopDomain,
    string Status,
    string Currency,
    string Language,
    string Timezone,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    Guid SellerId,
    string? SellerEmail,
    string? SellerName,
    string? ActivePlan,
    string? SubscriptionStatus);

public record AdminSubscriptionsResponse(
    AdminSubscriptionSummaryResponse Summary,
    PagedResponse<AdminSubscriptionResponse> Subscriptions,
    IReadOnlyList<AdminPlanResponse> Plans);

public record AdminSubscriptionSummaryResponse(
    int ActiveSubscriptions,
    decimal MonthlyRecurringRevenue,
    int TotalPlans);

public record AdminSubscriptionResponse(
    Guid Id,
    string Status,
    DateTime? RenewalDate,
    DateTime CreatedAt,
    Guid UserId,
    string? SellerEmail,
    Guid PlanId,
    string? PlanName,
    decimal PlanPrice);

public record AdminPlanResponse(
    Guid Id,
    string PlanName,
    string PlanDescription,
    string PlanStatus,
    decimal PlanPrice,
    int ActiveSubscriptions);

public record AdminAiAnalyticsResponse(
    long TotalMessages,
    long HighIntentMessages,
    decimal ConversionRate,
    IReadOnlyList<CountBreakdownItem> TopIntents,
    IReadOnlyList<CountBreakdownItem> SentimentBreakdown,
    AdminProductStatsResponse ProductStats,
    AdminMongoHealthResponse MongoHealth);

public record AdminProductStatsResponse(
    long TotalProducts,
    long TotalCategories,
    IReadOnlyList<CountBreakdownItem> TopCategories);

public record AdminMongoHealthResponse(
    string Status,
    string Database,
    long LatencyMs,
    double Ok,
    DateTime CheckedAt);

public record AdminAuditLogResponse(
    Guid Id,
    Guid? UserId,
    string Action,
    string? IpAddress,
    string? UserAgent,
    string? Metadata,
    DateTime CreatedAt);

internal sealed record AdminAiStats(
    long TotalMessages,
    long HighIntentMessages,
    decimal ConversionRate,
    List<CountBreakdownItem> TopIntents,
    List<CountBreakdownItem> SentimentBreakdown);

internal sealed record AdminMonthlyRevenuePoint(int Year, int Month, decimal Revenue);

internal sealed record AdminMonthlyConversationPoint(int Year, int Month, long Conversations);
