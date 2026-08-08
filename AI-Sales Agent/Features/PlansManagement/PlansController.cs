using MediatR;
using Microsoft.AspNetCore.Mvc;
using AI_Sales_Agent.Features.Plans.CreatePlan;
using AI_Sales_Agent.Features.Plans.UpdatePlan;
using AI_Sales_Agent.Features.Plans.DeletePlan;
using AI_Sales_Agent.Features.Plans.GetAllPlans;
using AI_Sales_Agent.Features.Plans.GetPlanById;

using Microsoft.AspNetCore.Authorization;
using AI_Sales_Agent.Infrastructure.Auth;
using AI_Sales_Agent.Features.Plans.GetPlanDevelopmentPrice;

namespace AI_Sales_Agent.Controllers;

[ApiController]
[Route("api/admin/plans")]
public class PlansAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlansAdminController(IMediator mediator) => _mediator = mediator;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _mediator.Send(new GetAllPlansQuery());
        return Ok(plans);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var plan = await _mediator.Send(new GetPlanByIdQuery(id));
        return plan != null ? Ok(plan) : NotFound("Plan not found.");
    } 
    [AllowAnonymous]
    [HttpGet("{id}/development-price")]
    public async Task<IActionResult> GetPlanDevelopmentPrice([FromRoute] Guid id)
    {
        var query = new GetPlanDevelopmentPriceQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { Message = "Plan not found or has been deleted." });

        return Ok(result);
    }

    [Authorize(Roles = Roles.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanCommand command)
    {
        var planId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = planId }, new { Message = "Plan created successfully", PlanId = planId });
    }

    [Authorize(Roles = Roles.SuperAdmin)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound("Plan not found or soft-deleted.");
    }


    [Authorize(Roles = Roles.SuperAdmin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeletePlanCommand(id));
        return result ? NoContent() : NotFound("Plan not found or already deleted.");
    }
}