using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.AuditLogs.GetAdminAuditLogs;

public record GetAdminAuditLogsQuery(
    string? Action,
    int Page,
    int PageSize) : IRequest<PagedResponse<AdminAuditLogResponse>>;
