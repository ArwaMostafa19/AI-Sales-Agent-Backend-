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
            .Cascade(CascadeMode.Stop) // لو فشل في NotEmpty هيقف ومش هينفذ MustAsync
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
}