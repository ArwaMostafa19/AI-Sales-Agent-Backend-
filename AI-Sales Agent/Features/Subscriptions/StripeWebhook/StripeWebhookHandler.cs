using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace AI_Sales_Agent.Features.Subscriptions.StripeWebhook;

public class StripeWebhookHandler
    : IRequestHandler<StripeWebhookCommand>
{
    private readonly ApplicationDbContext _context;

    public StripeWebhookHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        StripeWebhookCommand request,
        CancellationToken cancellationToken)
    {
        switch (request.StripeEvent.Type)
        {
            case "checkout.session.completed":
                await HandleCheckoutCompleted(
                    request.StripeEvent,
                    cancellationToken);
                break;

            case "customer.subscription.deleted":
                await HandleSubscriptionDeleted(
                    request.StripeEvent,
                    cancellationToken);
                break;

            case "invoice.payment_failed":
                await HandlePaymentFailed(
                    request.StripeEvent,
                    cancellationToken);
                break;
        }
    }

    private async Task HandleCheckoutCompleted(
        Event stripeEvent,
        CancellationToken cancellationToken)
    {
        var session = stripeEvent.Data.Object as Session;

        if (session == null)
            return;

        if (!session.Metadata.TryGetValue("UserId", out var userIdValue))
            throw new BadHttpRequestException("UserId metadata is missing.");

        if (!session.Metadata.TryGetValue("PlanId", out var planIdValue))
            throw new BadHttpRequestException("PlanId metadata is missing.");

        var userId = Guid.Parse(userIdValue);
        var planId = Guid.Parse(planIdValue);

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(
                s => s.UserId == userId,
                cancellationToken);

        if (subscription == null)
        {
            subscription = new AI_Sales_Agent.Domain.Subscription
            {
                UserId = userId,
                PlanId = planId,
                Status = "Active",
                RenewalDate = DateTime.UtcNow.AddMonths(1),

                StripeCustomerId = session.CustomerId,
                StripeSubscriptionId = session.SubscriptionId
            };

            _context.Subscriptions.Add(subscription);
        }
        else
        {
            subscription.PlanId = planId;
            subscription.Status = "Active";
            subscription.RenewalDate = DateTime.UtcNow.AddMonths(1);
            subscription.UpdatedAt = DateTime.UtcNow;

            subscription.StripeCustomerId = session.CustomerId;
            subscription.StripeSubscriptionId = session.SubscriptionId;

            subscription.IsTrial = false;

            subscription.TrialStartDate = null;

            subscription.TrialEndDate = null;
        }

        var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == subscription.UserId);
        user.HasUsedTrial = true;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleSubscriptionDeleted(
        Event stripeEvent,
        CancellationToken cancellationToken)
    {
        var stripeSubscription =
            stripeEvent.Data.Object as Stripe.Subscription;

        if (stripeSubscription == null)
            return;

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(
                s => s.StripeSubscriptionId == stripeSubscription.Id,
                cancellationToken);

        if (subscription == null)
            return;

        subscription.Status = "Cancelled";
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task HandlePaymentFailed(
        Event stripeEvent,
        CancellationToken cancellationToken)
    {
        var invoice =
            stripeEvent.Data.Object as Invoice;

        if (invoice == null)
            return;

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(
                s => s.StripeCustomerId == invoice.CustomerId,
                cancellationToken);

        if (subscription == null)
            return;

        subscription.Status = "PaymentFailed";
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}