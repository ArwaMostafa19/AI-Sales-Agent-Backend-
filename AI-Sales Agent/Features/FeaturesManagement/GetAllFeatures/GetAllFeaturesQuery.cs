using MediatR;

namespace AI_Sales_Agent.Features.FeaturesManagement.GetAllFeatures;

public record GetAllFeaturesQuery() : IRequest<List<FeatureListResponseDto>>;

public record FeatureListResponseDto(Guid Id, string Name, string Description, bool Enabled);