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
            .GreaterThan(0).WithMessage("Plan price must be grater than 0.");

        RuleFor(x => x.NumOfTokens)
            .GreaterThanOrEqualTo(0).WithMessage("Number of tokens cannot be negative.");

        RuleFor(x => x.TrialDays)
            .GreaterThanOrEqualTo(0).WithMessage("Number of Free Trial days cannot be negative.");

        RuleFor(x => x.Developmentprice)
            .GreaterThan(0).WithMessage("Development price must be grater than 0.");

        RuleFor(x => x.AiModels)
            .NotEmpty().WithMessage("At least one AI model must be assigned to the plan.");

        RuleFor(x => x.FeatureIds)
            .NotEmpty().WithMessage("At least one feature must be selected for this plan.");

        RuleForEach(x => x.FeatureIds)
            .NotEqual(Guid.Empty).WithMessage("Invalid Feature Id.");
    }
}