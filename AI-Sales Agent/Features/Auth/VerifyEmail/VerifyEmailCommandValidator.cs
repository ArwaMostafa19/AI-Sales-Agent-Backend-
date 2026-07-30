using FluentValidation;

namespace AI_Sales_Agent.Features.Auth.VerifyEmail
{
    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Token).NotEmpty();
        }
    }
}
