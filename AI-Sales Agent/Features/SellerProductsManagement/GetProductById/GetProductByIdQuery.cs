using MediatR;

namespace AI_Sales_Agent.Features.SellerProductsManagement.GetProductById;

public record GetProductByIdQuery(string ProductId, string StoreId) : IRequest<ProductDetailsDto?>;

public class ProductDetailsDto
{
    public string Id { get; set; } = string.Empty;
    public string StoreId { get; set; } = string.Empty;
    public string? OrganizationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "draft";
    public string? Vendor { get; set; }
    public List<string> Tags { get; set; } = new();
    public double Price { get; set; }
    public int Stock { get; set; }
    public double MaxAllowedDiscount { get; set; }
    public string? CategoryId { get; set; }
    public List<string> ImageUrls { get; set; } = new();
}
