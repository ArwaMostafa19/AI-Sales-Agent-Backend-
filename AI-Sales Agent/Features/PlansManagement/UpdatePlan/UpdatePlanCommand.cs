using MediatR;
using System.Text.Json.Serialization; 
namespace AI_Sales_Agent.Features.Plans.UpdatePlan;

public record UpdatePlanCommand(
    string PlanName,
    string PlanDescription,
    string PlanStatus,
    decimal PlanPrice,
    List<Guid> FeatureIds) : IRequest<bool>
{
    [JsonIgnore] 
    public Guid Id { get; set; }
}