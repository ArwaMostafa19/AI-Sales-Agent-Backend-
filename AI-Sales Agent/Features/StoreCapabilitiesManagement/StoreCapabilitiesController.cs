using AI_Sales_Agent.Features.SellerStoreManagement.GetStoreCapabilities;
using AI_Sales_Agent.Features.StoreCapabilitiesManagement;
using AI_Sales_Agent.Features.StoreCapabilitiesManagement.UpdateStoreCapabilities;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AI_Sales_Agent.Infrastructure.Auth;

namespace AI_Sales_Agent.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.SuperAdmin)] 
public class StoreCapabilitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public StoreCapabilitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("update-capabilities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCapabilities(
        [FromBody] UpdateStoreCapabilitiesCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
        {
            return BadRequest(new { Message = "Failed to update store capabilities." });
        }

        return Ok(new
        {
            Success = true,
            Message = "Store capabilities updated successfully."
        });
    }

    [HttpGet("{storeId}/capabilities")]
    public async Task<IActionResult> GetStoreCapabilities([FromRoute] string storeId)
    {
        var query = new GetStoreCapabilitiesQuery(storeId);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { Message = "Capabilities for this store were not found." });

        return Ok(result);
    }
}