using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerProductsManagement.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly MongoDbContext _context;

    public DeleteProductHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // 👈 ضفنا شرط (DeletedAt == null) عشان نضمن إن العملية تمشي فقط لو المنتج مش ممسوح سوفت قبل كده
        var filter = Builders<ProductDocument>.Filter.And(
            Builders<ProductDocument>.Filter.Eq(p => p.Id, request.ProductId),
            Builders<ProductDocument>.Filter.Eq(p => p.StoreId, request.StoreId),
            Builders<ProductDocument>.Filter.Eq(p => p.DeletedAt, null) // 👈 التعديل هنا
        );

        if (request.SoftDelete)
        {
            var update = Builders<ProductDocument>.Update
                .Set(p => p.DeletedAt, DateTime.UtcNow)
                .Set(p => p.Audit.UpdatedAt, DateTime.UtcNow); // أو p.UpdatedAt حسب التعريف عندك

            var updateResult = await _context.Products.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            return updateResult.ModifiedCount > 0;
        }

        var deleteResult = await _context.Products.DeleteOneAsync(filter, cancellationToken: cancellationToken);
        return deleteResult.DeletedCount > 0;
    }
}