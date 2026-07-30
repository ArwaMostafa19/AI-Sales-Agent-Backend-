using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerProductsManagement.UpdateMaxDiscount;

public class UpdateMaxDiscountHandler : IRequestHandler<UpdateMaxDiscountCommand, bool>
{
    private readonly MongoDbContext _context;

    public UpdateMaxDiscountHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateMaxDiscountCommand request, CancellationToken cancellationToken)
    {
        var filter = Builders<ProductDocument>.Filter.And(
            Builders<ProductDocument>.Filter.Eq(p => p.Id, request.ProductId),
            Builders<ProductDocument>.Filter.Eq(p => p.StoreId, request.StoreId)
        );

        var update = Builders<ProductDocument>.Update
            .Set(p => p.MaxAllowedDiscount, request.MaxAllowedDiscount)
            .Set(p => p.Audit.UpdatedAt, DateTime.UtcNow);

        var result = await _context.Products.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}