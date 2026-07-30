using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.StoreCapabilitiesManagement.UpdateStoreCapabilities;

public class UpdateStoreCapabilitiesHandler : IRequestHandler<UpdateStoreCapabilitiesCommand, bool>
{
    private readonly MongoDbContext _context;

    public UpdateStoreCapabilitiesHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateStoreCapabilitiesCommand request, CancellationToken cancellationToken)
    {
        string storeIdStr = request.StoreId.ToLower();

        var filter = Builders<StoreCapabilitiesDocument>.Filter.Eq(s => s.StoreId, storeIdStr);

        var existingDoc = await _context.StoreCapabilities.Find(filter).FirstOrDefaultAsync(cancellationToken);

        var capabilitiesBson = existingDoc?.Capabilities ?? new BsonDocument();
        capabilitiesBson["has_promo_code"] = request.HasPromoCode;

        var update = Builders<StoreCapabilitiesDocument>.Update
            .Set(s => s.StoreId, storeIdStr)
            .Set(s => s.Capabilities, capabilitiesBson)
            .Set(s => s.UpdatedAt, DateTime.UtcNow);

        var result = await _context.StoreCapabilities.UpdateOneAsync(
            filter,
            update,
            new UpdateOptions { IsUpsert = true },
            cancellationToken);

        // 💡 فلو منطقي: لو لغى البرومو كود، بنصّفر الخصومات القديمة لكل المنتجات
        if (!request.HasPromoCode)
        {
            var productFilter = Builders<ProductDocument>.Filter.Eq(p => p.StoreId, storeIdStr);
            var resetDiscountUpdate = Builders<ProductDocument>.Update
                .Set(p => p.MaxAllowedDiscount, 0)
                .Set(p => p.Audit.UpdatedAt, DateTime.UtcNow);

            await _context.Products.UpdateManyAsync(productFilter, resetDiscountUpdate, cancellationToken: cancellationToken);
        }

        return result.ModifiedCount > 0 || result.UpsertedId != null;
    }
}