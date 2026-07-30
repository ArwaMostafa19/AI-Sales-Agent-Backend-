using MediatR;

namespace AI_Sales_Agent.Features.SellerProductsManagement.CreateProduct;

public record CreateProductCommand(
    string StoreId,
    string? OrganizationId,
    string Title,
    string? Description,
    double Price,
    int Stock,
    double MaxAllowedDiscount = 0,
    string Status = "draft",
    string? CategoryId = null,
    string? Vendor = null,
    List<string>? Tags = null,
    List<string>? ImageUrls = null
) : IRequest<string>;
