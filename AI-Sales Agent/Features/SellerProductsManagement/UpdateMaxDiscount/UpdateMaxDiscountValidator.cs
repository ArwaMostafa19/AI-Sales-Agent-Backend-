using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.SellerProductsManagement.UpdateMaxDiscount;

public class UpdateMaxDiscountCommandValidator : AbstractValidator<UpdateMaxDiscountCommand>
{
    private readonly MongoDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateMaxDiscountCommandValidator(MongoDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required.")
            .Must(BeAuthorizedStore).WithMessage("Unauthorized: You can only update discount for your own store.");

        RuleFor(x => x.MaxAllowedDiscount)
            .InclusiveBetween(0, 100).WithMessage("Discount must be 0-100.");

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

    private async Task<bool> HasDiscountPermissionIfDiscountApplied(UpdateMaxDiscountCommand command, CancellationToken ct)
    {
        if (command.MaxAllowedDiscount <= 0) return true;

        string storeIdStr = command.StoreId.ToLower();
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