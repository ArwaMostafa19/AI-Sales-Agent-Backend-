using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateCategoryValidator(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category ID is required.");
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required.")
            .Must(BeAuthorizedStore).WithMessage("Unauthorized for this store.");

        RuleFor(x => x.Name).NotEmpty().WithMessage("Category name is required.");
    }

    private bool BeAuthorizedStore(string storeId)
    {
        var userStoreId = _httpContextAccessor.HttpContext?.User?.FindFirst("StoreId")?.Value
                       ?? _httpContextAccessor.HttpContext?.User?.FindFirst("store_id")?.Value;

        if (string.IsNullOrEmpty(userStoreId)) return true;
        return string.Equals(userStoreId, storeId, StringComparison.OrdinalIgnoreCase);
    }
}