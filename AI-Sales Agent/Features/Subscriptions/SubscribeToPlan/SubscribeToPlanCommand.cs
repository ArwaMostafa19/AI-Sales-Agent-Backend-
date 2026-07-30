using MediatR;
using System.Text.Json.Serialization;

namespace AI_Sales_Agent.Features.Subscriptions.SubscribeToPlan;

public record SubscribeToPlanCommand(Guid PlanId) : IRequest<SubscriptionResponseDto>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
}

public record SubscriptionResponseDto(
    Guid SubscriptionId,
    string Status,
    DateTime? RenewalDate,
    Guid PlanId,
    string PlanName,
    decimal PlanPrice
);