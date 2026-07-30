using FluentValidation;
using System;

namespace AI_Sales_Agent.Features.Plans.CreatePlan;

public class CreatePlanValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanValidator()
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
            .NotEmpty().WithMessage("At least one feature must be selected for this plan.");

        RuleForEach(x => x.FeatureIds)
            .NotEqual(Guid.Empty).WithMessage("Invalid Feature Id.");
    }
}