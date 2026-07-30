using AI_Sales_Agent.Features.DashboardManagement.GetTotalRevenue;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sales_Agent.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize(Roles = Roles.Seller)] // للـ Seller عشان يقدر يشوف أرباح متجره
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
}   