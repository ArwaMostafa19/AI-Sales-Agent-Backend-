using MediatR;

namespace AI_Sales_Agent.Features.SellerProductsManagement.UpdateProduct;

public record UpdateProductCommand(
    string ProductId,
    string StoreId,
    string Title,
    string? Description,
    double Price,
    int Stock,
    double MaxAllowedDiscount,
    string Status,
    string? CategoryId = null,
    string? Vendor = null,
    List<string>? Tags = null,
    List<string>? ImageUrls = null
) : IRequest<bool>;