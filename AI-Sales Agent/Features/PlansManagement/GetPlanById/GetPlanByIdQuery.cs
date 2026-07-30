using MediatR;

namespace AI_Sales_Agent.Features.Plans.GetPlanById;

public record GetPlanByIdQuery(Guid Id) : IRequest<PlanDetailsResponseDto?>;

public record PlanDetailsResponseDto(
    Guid Id,
    string PlanName,
    string PlanDescription,
    string PlanStatus,
    decimal PlanPrice,
    List<PlanFeatureDetailsDto> Features);

public record PlanFeatureDetailsDto(
    Guid FeatureId,
    string FeatureName,
    string FeatureDescription,
    bool Enabled);