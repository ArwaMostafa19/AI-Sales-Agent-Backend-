using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminRevenueGrowth;

public record GetAdminRevenueGrowthQuery : IRequest<IReadOnlyList<AdminGrowthPointResponse>>;
