using MediatR;

namespace AI_Sales_Agent.Features.SellerProductsManagement.GetAllProducts;

public record GetAllProductsQuery(
    string StoreId,
    int PageIndex = 1,
    int PageSize = 5,
    string? SearchTerm = null,
    string? Status = null
) : IRequest<PaginatedResult<ProductListItemDto>>;

public class ProductListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Stock { get; set; }
    public double MaxAllowedDiscount { get; set; }
    public string Status { get; set; } = "draft";
    public string? MainImageUrl { get; set; }
    public string? CategoryId { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageIndex < TotalPages;
    public bool HasPreviousPage => PageIndex > 1;

    public PaginatedResult(List<T> items, long count, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}