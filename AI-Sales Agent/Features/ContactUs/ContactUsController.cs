using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sales_Agent.Features.ContactUs.SendContactRequest;

[ApiController]
[Route("api/contact")]
[Authorize(Roles = Roles.Admin)]
public class ContactController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Send(
        SendContactRequestCommand command,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);

        return Ok(new
        {
            message = "Your request has been sent successfully."
        });
    }
}