using FluentValidation;

namespace AI_Sales_Agent.Features.Subscriptions.SubscribeToPlan;

public class SubscribeToPlanValidator : AbstractValidator<SubscribeToPlanCommand>
{
    public SubscribeToPlanValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty().WithMessage("Plan ID is required.");
    }
}