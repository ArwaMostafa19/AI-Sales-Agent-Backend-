using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminSystemHealth;

public record GetAdminSystemHealthQuery : IRequest<AdminMongoHealthResponse>;
