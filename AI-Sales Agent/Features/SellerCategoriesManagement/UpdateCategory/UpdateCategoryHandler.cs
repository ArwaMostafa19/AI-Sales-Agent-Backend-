using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
    private readonly MongoDbContext _context;

    public UpdateCategoryHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // نضمن عدم تعديل أي كاتجوري deletedAt بتاعها مش null
        var filter = Builders<CategoryDocument>.Filter.And(
            Builders<CategoryDocument>.Filter.Eq(c => c.Id, request.CategoryId),
            Builders<CategoryDocument>.Filter.Eq(c => c.StoreId, request.StoreId),
            Builders<CategoryDocument>.Filter.Eq(c => c.DeletedAt, null)
        );

        var update = Builders<CategoryDocument>.Update
            .Set(c => c.Name, request.Name)
            .Set(c => c.Description, request.Description)
            .Set(c => c.Handle, request.Name.ToLower().Trim().Replace(" ", "-"))
            .Set(c => c.ParentId, request.ParentId)
            .Set(c => c.ImageUrl, request.ImageUrl)
            .Set(c => c.SortOrder, request.SortOrder)
            .Set(c => c.Audit.UpdatedAt, DateTime.UtcNow);

        var result = await _context.Categories.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}