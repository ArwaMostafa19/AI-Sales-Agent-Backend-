using AI_Sales_Agent.Features.Admin.Shared;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminSentimentBreakdown;

public class GetAdminSentimentBreakdownHandler : IRequestHandler<GetAdminSentimentBreakdownQuery, IReadOnlyList<CountBreakdownItem>>
{
    private readonly IMongoDbContext _mongoDbContext;

    public GetAdminSentimentBreakdownHandler(IMongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<IReadOnlyList<CountBreakdownItem>> Handle(
        GetAdminSentimentBreakdownQuery request,
        CancellationToken cancellationToken)
    {
        var aiStats = await AdminDashboardQueryHelpers.GetAiStats(_mongoDbContext, cancellationToken);
        return aiStats.SentimentBreakdown;
    }
}
