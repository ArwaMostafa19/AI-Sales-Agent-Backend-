using System.Net;
using System.Text;
using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Email;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Features.Auth.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IAuditLogger _auditLogger;

        public ForgotPasswordCommandHandler(
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

        public async Task<ApiResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetUrl = BuildResetUrl(user.Email!, token);

                await _emailSender.SendAsync(
                    user.Email!,
                    "Reset your password",
                    $"<p>Reset your password here: <a href=\"{resetUrl}\">Reset password</a></p>",
                    cancellationToken);

                await _auditLogger.LogAsync("Auth.ForgotPassword", user.Id, cancellationToken: cancellationToken);
            }

            return ApiResult.Success("If the email exists, a password reset message has been sent.");
        }

        private string BuildResetUrl(string email, string token)
        {
            var baseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/');
            var encodedEmail = WebUtility.UrlEncode(email);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return $"/reset-password?email={encodedEmail}&token={encodedToken}";
            }

            return $"{baseUrl}/reset-password?email={encodedEmail}&token={encodedToken}";
        }
    }
}
