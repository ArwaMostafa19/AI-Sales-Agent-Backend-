using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerProductsManagement.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDetailsDto?>
{
    private readonly MongoDbContext _context;

    public GetProductByIdHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailsDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var filterBuilder = Builders<ProductDocument>.Filter;

        // 👈 الشرط: DeletedAt == null مباشرة
        var filter = filterBuilder.And(
            filterBuilder.Eq(p => p.Id, request.ProductId),
            filterBuilder.Eq(p => p.StoreId, request.StoreId),
            filterBuilder.Eq(p => p.DeletedAt, null)
        );

        var product = await _context.Products.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (product == null) return null;

        return new ProductDetailsDto
        {
            Id = product.Id,
            StoreId = product.StoreId,
            OrganizationId = product.OrganizationId,
            Title = product.Title,
            Description = product.Description,
            Status = product.Status,
            Vendor = product.Vendor,
            Tags = product.Tags ?? new List<string>(),
            Price = (double)(product.Variants.FirstOrDefault()?.Price?.Amount ?? 0),
            Stock = product.Variants.Sum(v => v.InventoryQuantity),
            MaxAllowedDiscount = product.MaxAllowedDiscount,
            CategoryId = product.CategoryId,
            ImageUrls = product.Images.Select(i => i.Url).ToList()
        };
    }
}