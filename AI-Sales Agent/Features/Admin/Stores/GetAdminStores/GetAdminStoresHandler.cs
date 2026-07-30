using AI_Sales_Agent.Data;
using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Admin.Stores.GetAdminStores;

public class GetAdminStoresHandler : IRequestHandler<GetAdminStoresQuery, PagedResponse<AdminStoreResponse>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetAdminStoresHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<AdminStoreResponse>> Handle(
        GetAdminStoresQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var query = AdminDashboardQueryHelpers.BuildStoresQuery(
            _dbContext,
            request.Search,
            request.Platform,
            request.Status);

        var total = await query.CountAsync(cancellationToken);
        var stores = await query
            .OrderByDescending(store => store.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .SelectAdminStoreResponse()
            .ToListAsync(cancellationToken);

        return new PagedResponse<AdminStoreResponse>(stores, page, pageSize, total);
    }
}
