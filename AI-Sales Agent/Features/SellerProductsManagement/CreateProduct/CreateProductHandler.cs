using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;

namespace AI_Sales_Agent.Features.SellerProductsManagement.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, string>
{
    private readonly MongoDbContext _context;

    public CreateProductHandler(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new ProductDocument
        {
            StoreId = request.StoreId,
            OrganizationId = request.OrganizationId,
            Title = request.Title,
            Description = request.Description,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "draft" : request.Status,
            MaxAllowedDiscount = request.MaxAllowedDiscount,
            CategoryId = request.CategoryId,
            Vendor = request.Vendor,
            Tags = request.Tags ?? new List<string>(),
            Variants = new List<VariantModel>
            {
                new VariantModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Default",
                    Price = new MoneyModel { Amount = (double)request.Price, Currency = "USD" }, // 👈 Default Currency from Schema
                    InventoryQuantity = request.Stock
                }
            },
            Images = request.ImageUrls?.Select((url, index) => new ImageModel
            {
                Url = url,
                Position = index + 1
            }).ToList() ?? new List<ImageModel>(),
            Audit = new AuditInfoModel
            {
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Products.InsertOneAsync(product, cancellationToken: cancellationToken);
        return product.Id;
    }
}