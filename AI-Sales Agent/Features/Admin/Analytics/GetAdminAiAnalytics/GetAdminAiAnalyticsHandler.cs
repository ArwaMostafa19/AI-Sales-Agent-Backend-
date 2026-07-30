using AI_Sales_Agent.Features.Admin.Shared;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Analytics.GetAdminAiAnalytics;

public class GetAdminAiAnalyticsHandler : IRequestHandler<GetAdminAiAnalyticsQuery, AdminAiAnalyticsResponse>
{
    private readonly IMongoDbContext _mongoDbContext;

    public GetAdminAiAnalyticsHandler(IMongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<AdminAiAnalyticsResponse> Handle(
        GetAdminAiAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        var aiStats = await AdminDashboardQueryHelpers.GetAiStats(_mongoDbContext, cancellationToken);
        var mongoHealth = await AdminDashboardQueryHelpers.GetMongoHealth(_mongoDbContext, cancellationToken);
        var productStats = await AdminDashboardQueryHelpers.GetProductStats(_mongoDbContext, cancellationToken);

        return new AdminAiAnalyticsResponse(
            aiStats.TotalMessages,
            aiStats.HighIntentMessages,
            aiStats.ConversionRate,
            aiStats.TopIntents,
            aiStats.SentimentBreakdown,
            productStats,
            mongoHealth);
    }
}
