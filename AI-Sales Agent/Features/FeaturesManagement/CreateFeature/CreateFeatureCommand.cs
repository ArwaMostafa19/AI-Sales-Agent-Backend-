using MediatR;

namespace AI_Sales_Agent.Features.FeaturesManagement.CreateFeature;

public record CreateFeatureCommand(string Name, string Description, bool Enabled) : IRequest<Guid>;