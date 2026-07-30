using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerProductsManagement.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    private readonly MongoDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CreateProductValidator(MongoDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required.")
            .Must(BeAuthorizedStore).WithMessage("You are not authorized to modify products in this store.");

        //RuleFor(x => x.OrganizationId)
        //    .NotEmpty().WithMessage("Organization ID is required.");

        RuleFor(x => x.CategoryId)
            .Cascade(CascadeMode.Stop) // لو فشل في NotEmpty هيقف ومش هينفذ MustAsync
            .NotEmpty().WithMessage("CategoryId is required.")
            .MustAsync(BeAnExistingAndActiveCategory)
            .WithMessage("The selected category does not exist or has been deleted.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Product title is required.")
            .MaximumLength(200).WithMessage("Product title cannot exceed 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Product price cannot be negative.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Product stock cannot be negative.");

        RuleFor(x => x.MaxAllowedDiscount)
            .InclusiveBetween(0, 100).WithMessage("Max allowed discount must be between 0% and 100%.");

        RuleFor(x => x)
            .MustAsync(HasDiscountPermissionIfDiscountApplied)
            .WithMessage("Promo code capabilities are disabled for this store. Max allowed discount must be 0.");
    }
    private async Task<bool> BeAnExistingAndActiveCategory(string? categoryId, CancellationToken cancellationToken)
    {
        // 1. لو فاضي أو null يرجع false على طول
        if (string.IsNullOrWhiteSpace(categoryId))
            return false;

        // 2. فحص إن الـ string عبارة عن 24 حرف Hex صح بتوع MongoDB ObjectId
        if (!MongoDB.Bson.ObjectId.TryParse(categoryId, out var objectId))
            return false;

        // 3. البحث في الداتا بيز بالـ objectId الصحيح
        var filter = Builders<CategoryDocument>.Filter.And(
            Builders<CategoryDocument>.Filter.Eq(c => c.Id, categoryId),
            Builders<CategoryDocument>.Filter.Eq(c => c.DeletedAt, null)
        );

        var count = await _context.Categories.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    private bool BeAuthorizedStore(string storeId)
    {
        var userStoreId = _httpContextAccessor.HttpContext?.User?.FindFirst("StoreId")?.Value
                       ?? _httpContextAccessor.HttpContext?.User?.FindFirst("store_id")?.Value;

        if (string.IsNullOrEmpty(userStoreId)) return true;
        return string.Equals(userStoreId, storeId, StringComparison.OrdinalIgnoreCase);
    }


    private async Task<bool> HasDiscountPermissionIfDiscountApplied(CreateProductCommand command, CancellationToken ct)
    {
        // لو الخصم المبعوث 0، ينفذ عادي بدون مشاكل
        if (command.MaxAllowedDiscount <= 0) return true;

        // لو مبعوث خصم > 0 نتحقق من الـ capabilities
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

        return false; // Default is false
    }
}