using AI_Sales_Agent.Data;
using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminRecentStores;

public class GetAdminRecentStoresHandler : IRequestHandler<GetAdminRecentStoresQuery, IReadOnlyList<AdminStoreResponse>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetAdminRecentStoresHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AdminStoreResponse>> Handle(
        GetAdminRecentStoresQuery request,
        CancellationToken cancellationToken)
    {
        var count = Math.Clamp(request.Count, 1, 50);
        return await AdminDashboardQueryHelpers.BuildStoresQuery(_dbContext, null, null, null)
            .OrderByDescending(store => store.CreatedAt)
            .Take(count)
            .SelectAdminStoreResponse()
            .ToListAsync(cancellationToken);
    }
}
