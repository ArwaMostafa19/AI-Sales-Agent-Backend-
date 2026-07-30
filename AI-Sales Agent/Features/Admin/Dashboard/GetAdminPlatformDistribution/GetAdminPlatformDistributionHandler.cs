using AI_Sales_Agent.Data;
using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminPlatformDistribution;

public class GetAdminPlatformDistributionHandler : IRequestHandler<GetAdminPlatformDistributionQuery, IReadOnlyList<CountBreakdownItem>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetAdminPlatformDistributionHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CountBreakdownItem>> Handle(
        GetAdminPlatformDistributionQuery request,
        CancellationToken cancellationToken)
    {
        var storesQuery = _dbContext.Stores.AsNoTracking().Where(store => store.DeletedAt == null);
        var totalStores = await storesQuery.CountAsync(cancellationToken);

        return await storesQuery
            .GroupBy(store => string.IsNullOrWhiteSpace(store.Platform) ? "Unknown" : store.Platform)
            .Select(group => new CountBreakdownItem(
                group.Key,
                group.Count(),
                AdminDashboardQueryHelpers.ToPercent(group.Count(), totalStores)))
            .ToListAsync(cancellationToken);
    }
}
