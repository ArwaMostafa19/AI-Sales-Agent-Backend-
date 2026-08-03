using FluentValidation;

namespace AI_Sales_Agent.Features.Subscriptions.StartFreeTrial;

public class StartFreeTrialValidator
    : AbstractValidator<StartFreeTrialCommand>
{
    public StartFreeTrialValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty()
            .WithMessage("Plan Id is required.");
    }
}