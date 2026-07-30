using AI_Sales_Agent.Domain.Mongo;
using MediatR;

namespace AI_Sales_Agent.Features.OrdersManagement.CreateOrder;

public record CreateOrderCommand(
    string StoreId,
    string OrgId,
    string? CustomerId,
    string? CustomerEmail,
    List<LineItemModel> LineItems,
    AddressModel? ShippingAddress,
    double TotalPriceAmount,
    string Currency = "USD",
    string FinancialStatus = "paid"
) : IRequest<string>;