using FluentValidation;

namespace AI_Sales_Agent.Features.Stores.UpdateStoreSettings
{
    public class UpdateStoreSettingsCommandValidator : AbstractValidator<UpdateStoreSettingsCommand>
    {
        public UpdateStoreSettingsCommandValidator()
        {
            RuleFor(command => command.StoreId).NotEmpty();
            RuleFor(command => command.Currency).NotEmpty().MaximumLength(10);
            RuleFor(command => command.Language).NotEmpty().MaximumLength(20);
            RuleFor(command => command.Timezone).NotEmpty().MaximumLength(100);
        }
    }
}
