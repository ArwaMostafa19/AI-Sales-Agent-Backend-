using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.DeleteCategory;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    private readonly MongoDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteCategoryValidator(MongoDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required.")
            .Must(BeAuthorizedStore).WithMessage("Unauthorized for this store.");

        // يمنع المسح (Soft أو Hard) لو الكاتجوري جواها منتجات مش ممسوحة
        RuleFor(x => x.CategoryId)
            .MustAsync(HasNoLinkedProducts)
            .WithMessage("Cannot delete category because it contains active products. Delete or reassign the products first.");
    }

    private async Task<bool> HasNoLinkedProducts(string categoryId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(categoryId)) return true;

        var productFilter = Builders<ProductDocument>.Filter.And(
            Builders<ProductDocument>.Filter.Eq(p => p.CategoryId, categoryId),
            Builders<ProductDocument>.Filter.Eq(p => p.DeletedAt, null)
        );

        var count = await _context.Products.CountDocumentsAsync(productFilter, cancellationToken: cancellationToken);
        return count == 0;
    }

    private bool BeAuthorizedStore(string storeId)
    {
        var userStoreId = _httpContextAccessor.HttpContext?.User?.FindFirst("StoreId")?.Value
                       ?? _httpContextAccessor.HttpContext?.User?.FindFirst("store_id")?.Value;

        if (string.IsNullOrEmpty(userStoreId)) return true;
        return string.Equals(userStoreId, storeId, StringComparison.OrdinalIgnoreCase);
    }
}