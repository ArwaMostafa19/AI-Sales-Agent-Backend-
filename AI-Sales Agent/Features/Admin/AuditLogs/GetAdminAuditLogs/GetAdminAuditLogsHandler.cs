using AI_Sales_Agent.Data;
using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Admin.AuditLogs.GetAdminAuditLogs;

public class GetAdminAuditLogsHandler : IRequestHandler<GetAdminAuditLogsQuery, PagedResponse<AdminAuditLogResponse>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetAdminAuditLogsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<AdminAuditLogResponse>> Handle(
        GetAdminAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var query = _dbContext.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Action))
        {
            query = query.Where(log => log.Action.Contains(request.Action));
        }

        var total = await query.CountAsync(cancellationToken);
        var logs = await query
            .OrderByDescending(log => log.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(log => new AdminAuditLogResponse(
                log.Id,
                log.UserId,
                log.Action,
                log.IpAddress,
                log.UserAgent,
                log.Metadata,
                log.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResponse<AdminAuditLogResponse>(logs, page, pageSize, total);
    }
}
