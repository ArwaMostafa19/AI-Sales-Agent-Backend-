using MediatR;
using System.Text.Json.Serialization;

namespace AI_Sales_Agent.Features.Subscriptions.CreateCheckoutSession;

public class CreateCheckoutSessionCommand
    : IRequest<CreateCheckoutSessionResponse>
{
    public Guid PlanId { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }
}

public record CreateCheckoutSessionResponse(string CheckoutUrl);