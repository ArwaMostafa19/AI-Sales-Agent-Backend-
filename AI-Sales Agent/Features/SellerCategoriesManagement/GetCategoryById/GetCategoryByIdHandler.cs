using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Features.SellerCategoriesManagement.GetAllCategories;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.GetCategoryById;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponseDto?>
{
    private readonly MongoDbContext _context;

    public GetCategoryByIdHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryResponseDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        // يتأكد إن deletedAt يساوي null
        var filter = Builders<CategoryDocument>.Filter.And(
            Builders<CategoryDocument>.Filter.Eq(c => c.Id, request.CategoryId),
            Builders<CategoryDocument>.Filter.Eq(c => c.StoreId, request.StoreId),
            Builders<CategoryDocument>.Filter.Eq(c => c.DeletedAt, null)
        );

        var c = await _context.Categories.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (c == null) return null;

        return new CategoryResponseDto(
            c.Id,
            c.StoreId,
            c.OrgId,
            c.Name,
            c.Description,
            c.Handle,
            c.ParentId,
            c.ImageUrl,
            c.SortOrder,
            c.ProductCount
        );
    }
}