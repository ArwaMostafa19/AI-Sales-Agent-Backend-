using FluentValidation;

namespace AI_Sales_Agent.Features.Plans.GetPlanDevelopmentPrice;

public class GetPlanDevelopmentPriceValidator : AbstractValidator<GetPlanDevelopmentPriceQuery>
{
    public GetPlanDevelopmentPriceValidator()
    {
        RuleFor(x => x.PlanId)
            .NotEmpty().WithMessage("Plan ID is required.");
    }
}