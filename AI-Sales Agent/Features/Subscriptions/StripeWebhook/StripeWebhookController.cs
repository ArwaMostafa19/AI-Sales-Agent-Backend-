using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace AI_Sales_Agent.Features.Subscriptions.StripeWebhook;

[ApiController]
[Route("api/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;

    public StripeWebhookController(
        IConfiguration configuration,
        IMediator mediator)
    {
        _configuration = configuration;
        _mediator = mediator;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body)
            .ReadToEndAsync();

        var signature =
            Request.Headers["Stripe-Signature"];

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _configuration["Stripe:WebhookSecret"]);
        }
        catch
        {
            return BadRequest();
        }

        await _mediator.Send(new StripeWebhookCommand
        {
            StripeEvent = stripeEvent
        });

        return Ok();
    }
}