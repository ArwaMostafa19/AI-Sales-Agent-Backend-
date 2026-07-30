using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace AI_Sales_Agent.Features.SellerProductsManagement.UpdateMaxDiscount;

public class UpdateMaxDiscountCommandValidator : AbstractValidator<UpdateMaxDiscountCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateMaxDiscountCommandValidator(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required.")
            .Must(BeAuthorizedStore).WithMessage("Unauthorized: You can only update discount for your own store.");

        RuleFor(x => x.MaxAllowedDiscount).InclusiveBetween(0, 100).WithMessage("Discount must be 0-100.");
    }

    private bool BeAuthorizedStore(string storeId)
    {
        var userStoreId = _httpContextAccessor.HttpContext?.User?.FindFirst("StoreId")?.Value
                       ?? _httpContextAccessor.HttpContext?.User?.FindFirst("store_id")?.Value;

        if (string.IsNullOrEmpty(userStoreId)) return true;
        return string.Equals(userStoreId, storeId, StringComparison.OrdinalIgnoreCase);
    }
}