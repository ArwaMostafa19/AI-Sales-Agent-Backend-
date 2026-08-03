namespace AI_Sales_Agent.Services;

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(
        Guid userId,
        Guid planId,
        string planName,
        string planDescription,
        decimal planPrice
        );

    Task CancelSubscriptionAsync(string stripeSubscriptionId);
}