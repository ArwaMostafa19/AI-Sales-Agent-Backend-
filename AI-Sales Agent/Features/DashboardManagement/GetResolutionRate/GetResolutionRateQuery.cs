using MediatR;

namespace AI_Sales_Agent.Features.DashboardManagement.GetResolutionRate;

public record GetResolutionRateQuery(Guid StoreId) : IRequest<ResolutionRateResponseDto>;

public record ResolutionRateResponseDto(
    double ResolutionRate,
    long TotalConversations,
    long ResolvedConversations
);