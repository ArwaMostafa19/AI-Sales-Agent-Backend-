using MediatR;

namespace AI_Sales_Agent.Features.FeaturesManagement.DeleteFeature;

public record DeleteFeatureCommand(Guid Id) : IRequest<bool>;