using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerProductsManagement.GetAllProducts;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, PaginatedResult<ProductListItemDto>>
{
    private readonly MongoDbContext _context;

    public GetAllProductsHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ProductListItemDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var filterBuilder = Builders<ProductDocument>.Filter;

        // 👈 الشرط: DeletedAt == null مباشرة
        var filter = filterBuilder.And(
            filterBuilder.Eq(p => p.StoreId, request.StoreId),
            filterBuilder.Eq(p => p.DeletedAt, null)
        );

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            filter &= filterBuilder.Regex(p => p.Title, new MongoDB.Bson.BsonRegularExpression(request.SearchTerm, "i"));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            filter &= filterBuilder.Eq(p => p.Status, request.Status);
        }

        var totalCount = await _context.Products.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var products = await _context.Products.Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = products.Select(p => new ProductListItemDto
        {
            Id = p.Id,
            Title = p.Title,
            Price = (double)(p.Variants.FirstOrDefault()?.Price?.Amount ?? 0),
            Stock = p.Variants.Sum(v => v.InventoryQuantity),
            MaxAllowedDiscount = p.MaxAllowedDiscount,
            Status = p.Status,
            MainImageUrl = p.Images.OrderBy(i => i.Position).FirstOrDefault()?.Url,
            CategoryId = p.CategoryId
        }).ToList();

        return new PaginatedResult<ProductListItemDto>(dtos, totalCount, request.PageIndex, request.PageSize);
    }
}