using AI_Sales_Agent.Infrastructure.Mongo;
using AI_Sales_Agent.Domain.Mongo;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerProductsManagement.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly MongoDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateProductCommandValidator(MongoDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required.")
            .Must(BeAuthorizedStore).WithMessage("You are not authorized to modify products in this store.");

        RuleFor(x => x.CategoryId)
            .Cascade(CascadeMode.Stop) 
            .NotEmpty().WithMessage("CategoryId is required.")
            .MustAsync(BeAnExistingAndActiveCategory)
            .WithMessage("The selected category does not exist or has been deleted.");
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Product title is required.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(x => x.MaxAllowedDiscount)
            .InclusiveBetween(0, 100).WithMessage("Max allowed discount must be between 0 and 100.");

        
        RuleFor(x => x)
            .MustAsync(MustBeExistingAndNotDeleted)
            .WithMessage("Product not found or has been deleted.");

        RuleFor(x => x)
            .MustAsync(HasDiscountPermissionIfDiscountApplied)
            .WithMessage("Promo code capabilities are disabled for this store. Max allowed discount must be 0.");
    }

    private bool BeAuthorizedStore(string storeId)
    {
        var userStoreId = _httpContextAccessor.HttpContext?.User?.FindFirst("StoreId")?.Value
                       ?? _httpContextAccessor.HttpContext?.User?.FindFirst("store_id")?.Value;

        if (string.IsNullOrEmpty(userStoreId)) return true;
        return string.Equals(userStoreId, storeId, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<bool> MustBeExistingAndNotDeleted(UpdateProductCommand command, CancellationToken ct)
    {
        var filter = Builders<ProductDocument>.Filter.And(
            Builders<ProductDocument>.Filter.Eq(p => p.Id, command.ProductId),
            Builders<ProductDocument>.Filter.Eq(p => p.StoreId, command.StoreId),
            Builders<ProductDocument>.Filter.Eq(p => p.DeletedAt, null) 
        );

        return await _context.Products.Find(filter).AnyAsync(ct);
    }

    private async Task<bool> BeAnExistingAndActiveCategory(string? categoryId, CancellationToken cancellationToken)
    {

        if (string.IsNullOrWhiteSpace(categoryId))
            return false;

       
        if (!MongoDB.Bson.ObjectId.TryParse(categoryId, out var objectId))
            return false;

       
        var filter = Builders<CategoryDocument>.Filter.And(
            Builders<CategoryDocument>.Filter.Eq(c => c.Id, categoryId),
            Builders<CategoryDocument>.Filter.Eq(c => c.DeletedAt, null)
        );

        var count = await _context.Categories.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    private async Task<bool> HasDiscountPermissionIfDiscountApplied(UpdateProductCommand command, CancellationToken ct)
    {
        if (command.MaxAllowedDiscount <= 0) return true;
        return await CheckHasPromoCodeCapabilityAsync(command.StoreId, ct);
    }

    private async Task<bool> CheckHasPromoCodeCapabilityAsync(string storeId, CancellationToken ct)
    {
        string storeIdStr = storeId.ToLower();
        var filter = Builders<StoreCapabilitiesDocument>.Filter.Eq(s => s.StoreId, storeIdStr);

        var doc = await _context.StoreCapabilities.Find(filter).FirstOrDefaultAsync(ct);
        if (doc == null || doc.Capabilities == null) return false;

        if (doc.Capabilities.Contains("has_promo_code") && doc.Capabilities["has_promo_code"].IsBoolean)
        {
            return doc.Capabilities["has_promo_code"].AsBoolean;
        }

        return false;
    }
}