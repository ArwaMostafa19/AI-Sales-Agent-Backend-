using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminSentimentBreakdown;

public record GetAdminSentimentBreakdownQuery : IRequest<IReadOnlyList<CountBreakdownItem>>;
