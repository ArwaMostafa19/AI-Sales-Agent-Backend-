using MediatR;
using System.Text.Json.Serialization;

namespace AI_Sales_Agent.Features.Subscriptions.CancelSubscription;

public record CancelSubscriptionCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
}