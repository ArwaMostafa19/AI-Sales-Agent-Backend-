using FluentValidation;

namespace AI_Sales_Agent.Features.Stores.DeleteStore
{
    public class DeleteStoreCommandValidator : AbstractValidator<DeleteStoreCommand>
    {
        public DeleteStoreCommandValidator()
        {
            RuleFor(command => command.StoreId).NotEmpty();
        }
    }
}
