using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.GetAllCategories;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryResponseDto>>
{
    private readonly MongoDbContext _context;

    public GetAllCategoriesHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryResponseDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var filterBuilder = Builders<CategoryDocument>.Filter;

        // شرط أساسي: DeletedAt يكون null
        var filter = filterBuilder.And(
            filterBuilder.Eq(c => c.StoreId, request.StoreId),
            filterBuilder.Eq(c => c.DeletedAt, null)
        );

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            filter &= filterBuilder.Regex(c => c.Name, new MongoDB.Bson.BsonRegularExpression(request.Search, "i"));
        }

        var categories = await _context.Categories.Find(filter)
            .SortBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);

        return categories.Select(c => new CategoryResponseDto(
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
        )).ToList();
    }
}