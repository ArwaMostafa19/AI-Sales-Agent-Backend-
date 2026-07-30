using MediatR;
using Microsoft.AspNetCore.Mvc;
using AI_Sales_Agent.Features.FeaturesManagement.CreateFeature;
using AI_Sales_Agent.Features.FeaturesManagement.UpdateFeature;
using AI_Sales_Agent.Features.FeaturesManagement.DeleteFeature;
using AI_Sales_Agent.Features.FeaturesManagement.GetAllFeatures;
using AI_Sales_Agent.Features.FeaturesManagement.GetFeatureById; // New Import

using Microsoft.AspNetCore.Authorization;
using AI_Sales_Agent.Infrastructure.Auth;

namespace AI_Sales_Agent.Controllers;

[ApiController]
// [Authorize(Roles = Roles.Admin)] // Temporarily disabled for admin endpoint testing.
[Route("api/admin/features")]
public class FeaturesAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeaturesAdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var features = await _mediator.Send(new GetAllFeaturesQuery());
        return Ok(features);
    }

    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var feature = await _mediator.Send(new GetFeatureByIdQuery(id));
        return feature != null ? Ok(feature) : NotFound("Feature not found or has been deleted.");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFeatureCommand command)
    {
        var featureId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = featureId }, new { Message = "Global Feature registered successfully", FeatureId = featureId });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFeatureCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound("Feature not found or soft-deleted.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteFeatureCommand(id));
        return result ? NoContent() : NotFound("Feature not found or already deleted.");
    }
}
