using AI_Sales_Agent.Features.DashboardManagement.GetAvgResponseTime;
using AI_Sales_Agent.Features.DashboardManagement.GetResolutionRate;
using AI_Sales_Agent.Features.DashboardManagement.GetRevenueGrowth;
using AI_Sales_Agent.Features.DashboardManagement.GetTotalRevenue;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sales_Agent.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize(Roles = Roles.Seller)] 
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("total-revenue")]
    public async Task<IActionResult> GetTotalRevenue([FromQuery] string storeId)
    {
        var query = new GetTotalRevenueQuery(storeId);
        var totalRevenue = await _mediator.Send(query);

        return Ok(new
        {
            StoreId = storeId,
            TotalRevenue = totalRevenue
        });
    }

   
    [HttpGet("revenue-growth")]
    public async Task<IActionResult> GetRevenueGrowth([FromQuery] Guid storeId)
    {
        var query = new GetRevenueGrowthQuery(storeId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("resolution-rate")]
    public async Task<IActionResult> GetResolutionRate([FromQuery] Guid storeId)
    {
        var query = new GetResolutionRateQuery(storeId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("avgResponseTime")]
    public async Task<IActionResult> GetConversationAnalytics([FromQuery] Guid storeId)
    {
        var query = new GetConversationAnalyticsQuery(storeId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}