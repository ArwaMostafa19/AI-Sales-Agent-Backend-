using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AI_Sales_Agent.Infrastructure.Auth;
using AI_Sales_Agent.Features.Subscriptions.SubscribeToPlan;
using AI_Sales_Agent.Features.Subscriptions.CancelSubscription;
using AI_Sales_Agent.Features.Subscriptions.GetUserSubscription;

namespace AI_Sales_Agent.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/seller/subscriptions")]
public class SubscriptionsSellerController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionsSellerController(IMediator mediator) => _mediator = mediator;

    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpGet("my-subscription")]
    public async Task<IActionResult> GetMySubscription()
    {
        var userId = GetUserIdFromToken();
        var subscription = await _mediator.Send(new GetUserSubscriptionQuery(userId));

        return subscription != null ? Ok(subscription) : NotFound("You do not have an active subscription.");
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribeOrChangePlan([FromBody] SubscribeToPlanCommand command)
    {
        command.UserId = GetUserIdFromToken();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelSubscription()
    {
        var userId = GetUserIdFromToken();
        var result = await _mediator.Send(new CancelSubscriptionCommand { UserId = userId });

        return result
            ? Ok(new { Message = "Subscription cancelled successfully." })
            : BadRequest("No active subscription found to cancel.");
    }
}