using FluentValidation;

namespace AI_Sales_Agent.Features.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(command => command.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(command => command.LastName).NotEmpty().MaximumLength(100);
            RuleFor(command => command.Email).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(command => command.Password).NotEmpty().MinimumLength(8);
            RuleFor(command => command.ConfirmPassword)
                .Equal(command => command.Password)
                .WithMessage("Passwords do not match.");
        }
    }
}
