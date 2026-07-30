using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace AI_Sales_Agent.Features.StoreCapabilitiesManagement.UpdateStoreCapabilities;

public class UpdateStoreCapabilitiesValidator : AbstractValidator<UpdateStoreCapabilitiesCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateStoreCapabilitiesValidator(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required.")
            .Must(BeAuthorizedStore).WithMessage("You are not authorized to modify capabilities for this store.");
    }

    private bool BeAuthorizedStore(string storeId)
    {
        var userStoreId = _httpContextAccessor.HttpContext?.User?.FindFirst("StoreId")?.Value
                       ?? _httpContextAccessor.HttpContext?.User?.FindFirst("store_id")?.Value;

        if (string.IsNullOrEmpty(userStoreId)) return true;
        return string.Equals(userStoreId, storeId, StringComparison.OrdinalIgnoreCase);
    }
}