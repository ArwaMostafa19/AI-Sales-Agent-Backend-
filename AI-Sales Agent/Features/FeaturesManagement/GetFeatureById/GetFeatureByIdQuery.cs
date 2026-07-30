using MediatR;

namespace AI_Sales_Agent.Features.FeaturesManagement.GetFeatureById;

public record GetFeatureByIdQuery(Guid Id) : IRequest<FeatureDetailsResponseDto?>;

public record FeatureDetailsResponseDto(
    Guid Id,
    string Name,
    string Description,
    bool Enabled);