using AI_Sales_Agent.Features.Admin.Shared;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminSystemHealth;

public class GetAdminSystemHealthHandler : IRequestHandler<GetAdminSystemHealthQuery, AdminMongoHealthResponse>
{
    private readonly IMongoDbContext _mongoDbContext;

    public GetAdminSystemHealthHandler(IMongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<AdminMongoHealthResponse> Handle(
        GetAdminSystemHealthQuery request,
        CancellationToken cancellationToken)
    {
        return await AdminDashboardQueryHelpers.GetMongoHealth(_mongoDbContext, cancellationToken);
    }
}
