using AI_Sales_Agent.Data;
using AI_Sales_Agent.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Subscriptions.CancelSubscription;

public class CancelSubscriptionHandler
    : IRequestHandler<CancelSubscriptionCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly IStripeService _stripeService;

    public CancelSubscriptionHandler(
        ApplicationDbContext context,
        IStripeService stripeService)
    {
        _context = context;
        _stripeService = stripeService;
    }

    public async Task<bool> Handle(
        CancelSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(
                s => s.UserId == request.UserId &&
                     s.DeletedAt == null,
                cancellationToken);

        if (subscription == null)
            return false;

        if (subscription.Status == "Cancelled")
            return false;

        if (string.IsNullOrWhiteSpace(subscription.StripeSubscriptionId))
            throw new BadHttpRequestException(
                "Stripe Subscription Id not found.");

        await _stripeService.CancelSubscriptionAsync(
            subscription.StripeSubscriptionId);

        // متعملش SaveChanges هنا
        // الـ Webhook هو اللي هيحدث الـ Database

        return true;
    }
}