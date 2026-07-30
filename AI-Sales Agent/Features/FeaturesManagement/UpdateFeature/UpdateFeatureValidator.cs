using FluentValidation;

namespace AI_Sales_Agent.Features.FeaturesManagement.UpdateFeature;

public class UpdateFeatureValidator : AbstractValidator<UpdateFeatureCommand>
{
    public UpdateFeatureValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty();
    }
}