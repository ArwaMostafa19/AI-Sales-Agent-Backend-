using AI_Sales_Agent.Features.OrdersManagement.CreateOrder;
using MediatR;
using AI_Sales_Agent.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sales_Agent.Controllers;

[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/v1/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(
            request.StoreId,
            request.OrgId,
            request.CustomerId,
            request.CustomerEmail,
            request.LineItems,
            request.ShippingAddress,
            request.TotalPriceAmount,
            request.Currency,
            request.FinancialStatus
        );

        var orderId = await _mediator.Send(command);

        return Ok(new
        {
            OrderId = orderId,
            Message = "Order created and total revenue updated successfully."
        });
    }
}

#region --- Request DTO ---

public record CreateOrderRequest(
    string StoreId,
    string OrgId,
    string? CustomerId,
    string? CustomerEmail,
    List<Domain.Mongo.LineItemModel> LineItems,
    Domain.Mongo.AddressModel? ShippingAddress,
    double TotalPriceAmount,
    string Currency = "USD",
    string FinancialStatus = "paid"
);

#endregion