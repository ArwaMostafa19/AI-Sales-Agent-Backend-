using FluentValidation;

namespace AI_Sales_Agent.Features.Subscriptions.CreateCheckoutSession;

public class CreateCheckoutSessionValidator
    : AbstractValidator<CreateCheckoutSessionCommand>
{
    public CreateCheckoutSessionValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty()
            .WithMessage("Plan Id is required.");
    }
}