using FluentValidation;

namespace AI_Sales_Agent.Features.Stores.CreateStore
{
    public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
    {
        public CreateStoreCommandValidator()
        {
            RuleFor(command => command.Name).NotEmpty().MaximumLength(150);
            RuleFor(command => command.Description).MaximumLength(1000);
            RuleFor(command => command.Platform).NotEmpty().MaximumLength(100);
            RuleFor(command => command.ShopDomain)
                .NotEmpty()
                .MaximumLength(250)
                .Matches(@"^(https?:\/\/)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$")
                .WithMessage("Shop domain must be a valid domain name (e.g., store.myshopify.com or example.com).");
            RuleFor(command => command.Currency).NotEmpty().MaximumLength(10);
            RuleFor(command => command.Language).NotEmpty().MaximumLength(20);
            RuleFor(command => command.Timezone).NotEmpty().MaximumLength(100);
        }
    }
}
