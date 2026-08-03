using MediatR;
using Microsoft.AspNetCore.Mvc;
using AI_Sales_Agent.Features.Plans.CreatePlan;
using AI_Sales_Agent.Features.Plans.UpdatePlan;
using AI_Sales_Agent.Features.Plans.DeletePlan;
using AI_Sales_Agent.Features.Plans.GetAllPlans;
using AI_Sales_Agent.Features.Plans.GetPlanById;

using Microsoft.AspNetCore.Authorization;
using AI_Sales_Agent.Infrastructure.Auth;

namespace AI_Sales_Agent.Controllers;

[ApiController]
[Authorize(Roles = Roles.SuperAdmin)]
[Route("api/admin/plans")]
public class PlansAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlansAdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _mediator.Send(new GetAllPlansQuery());
        return Ok(plans);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var plan = await _mediator.Send(new GetPlanByIdQuery(id));
        return plan != null ? Ok(plan) : NotFound("Plan not found.");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanCommand command)
    {
        var planId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = planId }, new { Message = "Plan created successfully", PlanId = planId });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound("Plan not found or soft-deleted.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeletePlanCommand(id));
        return result ? NoContent() : NotFound("Plan not found or already deleted.");
    }
}