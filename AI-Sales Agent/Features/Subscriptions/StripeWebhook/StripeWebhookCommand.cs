using MediatR;
using Stripe;

namespace AI_Sales_Agent.Features.Subscriptions.StripeWebhook;

public class StripeWebhookCommand : IRequest
{
    public Event StripeEvent { get; set; } = default!;
}