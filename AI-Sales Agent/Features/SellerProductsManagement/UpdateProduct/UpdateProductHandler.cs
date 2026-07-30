using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerProductsManagement.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly MongoDbContext _context;

    public UpdateProductHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var filter = Builders<ProductDocument>.Filter.And(
            Builders<ProductDocument>.Filter.Eq(p => p.Id, request.ProductId),
            Builders<ProductDocument>.Filter.Eq(p => p.StoreId, request.StoreId)
        );

        var update = Builders<ProductDocument>.Update
            .Set(p => p.Title, request.Title)
            .Set(p => p.Description, request.Description)
            .Set(p => p.Status, request.Status)
            .Set(p => p.MaxAllowedDiscount, request.MaxAllowedDiscount)
            .Set(p => p.CategoryId, request.CategoryId)
            .Set(p => p.Vendor, request.Vendor)
            .Set(p => p.Tags, request.Tags ?? new List<string>())
            .Set("variants.0.price.amount", (decimal)request.Price)
            .Set("variants.0.inventory_quantity", request.Stock)
            .Set(p => p.Images, request.ImageUrls?.Select((url, index) => new ImageModel
            {
                Url = url,
                Position = index + 1
            }).ToList() ?? new List<ImageModel>())
            .Set(p => p.Audit.UpdatedAt, DateTime.UtcNow);

        var result = await _context.Products.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}