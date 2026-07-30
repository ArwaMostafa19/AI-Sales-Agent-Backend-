using FluentValidation;

namespace AI_Sales_Agent.Features.Auth.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(command => command.Email).NotEmpty().EmailAddress();
        }
    }
}
