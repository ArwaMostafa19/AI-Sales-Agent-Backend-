using MediatR;

namespace AI_Sales_Agent.Features.SellerProductsManagement.UpdateMaxDiscount;

public record UpdateMaxDiscountCommand(
    string ProductId,
    string StoreId,
    double MaxAllowedDiscount
) : IRequest<bool>;