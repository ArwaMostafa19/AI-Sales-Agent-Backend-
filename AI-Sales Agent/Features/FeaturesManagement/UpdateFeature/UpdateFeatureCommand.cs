using MediatR;
using System.Text.Json.Serialization;

namespace AI_Sales_Agent.Features.FeaturesManagement.UpdateFeature;

public record UpdateFeatureCommand(
    string Name,
    string Description,
    bool Enabled) : IRequest<bool>
{
    [JsonIgnore]
    public Guid Id { get; set; }
}