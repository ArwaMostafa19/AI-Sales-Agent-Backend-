using AI_Sales_Agent.Features.Admin.Shared;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminTopIntents;

public class GetAdminTopIntentsHandler : IRequestHandler<GetAdminTopIntentsQuery, IReadOnlyList<CountBreakdownItem>>
{
    private readonly IMongoDbContext _mongoDbContext;

    public GetAdminTopIntentsHandler(IMongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<IReadOnlyList<CountBreakdownItem>> Handle(
        GetAdminTopIntentsQuery request,
        CancellationToken cancellationToken)
    {
        var aiStats = await AdminDashboardQueryHelpers.GetAiStats(_mongoDbContext, cancellationToken);
        return aiStats.TopIntents;
    }
}
