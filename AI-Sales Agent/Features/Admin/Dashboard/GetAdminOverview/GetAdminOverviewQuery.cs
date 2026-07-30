using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminOverview;

public record GetAdminOverviewQuery : IRequest<AdminOverviewResponse>;
