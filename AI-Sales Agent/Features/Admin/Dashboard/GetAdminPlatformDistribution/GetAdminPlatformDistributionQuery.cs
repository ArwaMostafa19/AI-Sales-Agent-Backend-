using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminPlatformDistribution;

public record GetAdminPlatformDistributionQuery : IRequest<IReadOnlyList<CountBreakdownItem>>;
