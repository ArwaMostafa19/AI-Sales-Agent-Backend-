using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly MongoDbContext _context;

    public DeleteCategoryHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // الفلتر بيشترط إن الكاتجوري ما تطلعش لو ممسوحة سوفت من قبل كده
        var filter = Builders<CategoryDocument>.Filter.And(
            Builders<CategoryDocument>.Filter.Eq(c => c.Id, request.CategoryId),
            Builders<CategoryDocument>.Filter.Eq(c => c.StoreId, request.StoreId),
            Builders<CategoryDocument>.Filter.Eq(c => c.DeletedAt, null)
        );

        // 1️⃣ Soft Delete
        if (request.SoftDelete)
        {
            var update = Builders<CategoryDocument>.Update
                .Set(c => c.DeletedAt, DateTime.UtcNow)
                .Set(c => c.Audit.UpdatedAt, DateTime.UtcNow);

            var updateResult = await _context.Categories.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            return updateResult.ModifiedCount > 0;
        }

        // 2️⃣ Hard Delete (مسح نهائي من Atlas)
        var deleteResult = await _context.Categories.DeleteOneAsync(filter, cancellationToken: cancellationToken);
        return deleteResult.DeletedCount > 0;
    }
}