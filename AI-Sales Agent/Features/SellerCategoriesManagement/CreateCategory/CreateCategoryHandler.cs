using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, string>
{
    private readonly MongoDbContext _context;

    public CreateCategoryHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new CategoryDocument
        {
            StoreId = request.StoreId,
            OrgId = request.OrgId ?? string.Empty,
            Name = request.Name,
            Description = request.Description,
            Handle = request.Name.ToLower().Trim().Replace(" ", "-"),
            ParentId = request.ParentId,
            ImageUrl = request.ImageUrl,
            SortOrder = request.SortOrder,
            ProductCount = 0,
            Audit = new AuditInfoModel { CreatedAt = DateTime.UtcNow }
        };

        await _context.Categories.InsertOneAsync(category, cancellationToken: cancellationToken);
        return category.Id;
    }
}