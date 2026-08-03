using AI_Sales_Agent.Infrastructure.Stripe;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace AI_Sales_Agent.Services;

public class StripeService : IStripeService
{
    private readonly StripeOptions _stripeOptions;

    public StripeService(IOptions<StripeOptions> stripeOptions)
    {
        _stripeOptions = stripeOptions.Value;

        StripeConfiguration.ApiKey = _stripeOptions.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(
        Guid userId,
        Guid planId,
        string planName,
        string planDescription,
        decimal planPrice
        )
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string>
            {
                "card",
               
            },
            Mode = "subscription",

            Locale = "auto",


            SuccessUrl = "https://ai-commerce-frontend-tau.vercel.app/checkout/success",

            CancelUrl = "https://ai-commerce-frontend-tau.vercel.app/checkout/failed",

            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,

                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",

                        UnitAmountDecimal = planPrice * 100,

                        Recurring = new SessionLineItemPriceDataRecurringOptions
                        {
                            Interval = "month"
                        },

                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = planName,
                            Description = planDescription
                        },



                    }
                }
            ],

            Metadata = new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "PlanId", planId.ToString() }
            }
        };

        var service = new SessionService();

        var session = await service.CreateAsync(options);

        return session.Url!;
    }



    public async Task CancelSubscriptionAsync(string stripeSubscriptionId)
    {
        var service = new Stripe.SubscriptionService();

        await service.CancelAsync(stripeSubscriptionId);
    }
}