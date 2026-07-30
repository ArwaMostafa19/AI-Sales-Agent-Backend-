using FluentValidation;

namespace AI_Sales_Agent.Features.FeaturesManagement.CreateFeature;

public class CreateFeatureValidator : AbstractValidator<CreateFeatureCommand>
{
    public CreateFeatureValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Feature name is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Feature description is required.");
    }
}