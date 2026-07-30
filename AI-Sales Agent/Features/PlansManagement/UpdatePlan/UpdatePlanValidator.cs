using FluentValidation;

namespace AI_Sales_Agent.Features.Plans.UpdatePlan;

public class UpdatePlanValidator : AbstractValidator<UpdatePlanCommand>
{
    public UpdatePlanValidator()
    {
        RuleFor(x => x.PlanName)
            .NotEmpty().WithMessage("Plan name is required.")
            .MaximumLength(100).WithMessage("Plan name cannot exceed 100 characters.");

        RuleFor(x => x.PlanDescription)
            .NotEmpty().WithMessage("Plan description is required.");

        RuleFor(x => x.PlanStatus)
            .NotEmpty().WithMessage("Plan status is required.");

        RuleFor(x => x.PlanPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Plan price cannot be negative.");

        RuleFor(x => x.FeatureIds)
            .NotEmpty().WithMessage("Plan must have at least one feature linked.");
    }
}