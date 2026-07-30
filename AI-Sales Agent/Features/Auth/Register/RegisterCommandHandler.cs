using System.Net;
using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Email;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Features.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IAuditLogger _auditLogger;

        public RegisterCommandHandler(
            UserManager<User> userManager,
            IEmailSender emailSender,
            IConfiguration configuration,
            IAuditLogger auditLogger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim();
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is not null)
            {
                return ApiResult.Failure("Registration failed.", new[] { "Email is already registered." });
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                UserName = email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return ApiResult.Failure("Registration failed.", result.Errors.Select(error => error.Description));
            }

            // Assign the Seller role
            await _userManager.AddToRoleAsync(user, AI_Sales_Agent.Infrastructure.Auth.Roles.Seller);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verifyUrl = BuildVerifyUrl(user.Id, token);

            await _emailSender.SendAsync(
                user.Email!,
                "Verify your email",
                $"<p>Welcome {WebUtility.HtmlEncode(user.FirstName)}.</p><p>Verify your email here: <a href=\"{verifyUrl}\">Verify email</a></p>",
                cancellationToken);

            await _auditLogger.LogAsync("Auth.Register", user.Id, cancellationToken: cancellationToken);

            return ApiResult.Success("Account registered. Please verify your email.");
        }

        private string BuildVerifyUrl(Guid userId, string token)
        {
            var baseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/');
            var encodedToken = WebUtility.UrlEncode(token);

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return $"/api/auth/verify-email?userId={userId}&token={encodedToken}";
            }

            return $"{baseUrl}/api/auth/verify-email?userId={userId}&token={encodedToken}";
        }
    }
}
