using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminTopIntents;

public record GetAdminTopIntentsQuery : IRequest<IReadOnlyList<CountBreakdownItem>>;
