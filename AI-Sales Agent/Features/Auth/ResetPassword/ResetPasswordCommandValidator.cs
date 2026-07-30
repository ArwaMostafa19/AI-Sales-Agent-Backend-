using FluentValidation;

namespace AI_Sales_Agent.Features.Auth.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(command => command.Email).NotEmpty().EmailAddress();
            RuleFor(command => command.Token).NotEmpty();
            RuleFor(command => command.NewPassword).NotEmpty().MinimumLength(8);
            RuleFor(command => command.ConfirmNewPassword)
                .Equal(command => command.NewPassword)
                .WithMessage("Passwords do not match.");
        }
    }
}
