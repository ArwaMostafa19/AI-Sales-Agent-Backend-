using FluentValidation;

namespace AI_Sales_Agent.Features.ContactUs.SendContactRequest;

public class SendContactRequestValidation
    : AbstractValidator<SendContactRequestCommand>
{
    public SendContactRequestValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.StoreName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StoreDescription)
            .MaximumLength(15000);

        RuleFor(x => x.Notes)
            .MaximumLength(15000);

        RuleFor(x => x.Message)
            .MaximumLength(5000);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(x => x.ContactPreference)
            .NotEmpty()
            .MaximumLength(100);
    }
}