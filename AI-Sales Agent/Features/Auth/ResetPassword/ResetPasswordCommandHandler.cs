using System.Net;
using System.Text;
using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Features.Auth.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuditLogger _auditLogger;
        private readonly IRefreshTokenService _refreshTokenService;

        public ResetPasswordCommandHandler(
            UserManager<User> userManager,
            IAuditLogger auditLogger,
            IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _auditLogger = auditLogger;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<ApiResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return ApiResult.Failure("Password reset failed.", new[] { "Invalid reset request." });
            }

            var token = DecodeResetToken(request.Token);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!result.Succeeded)
            {
                return ApiResult.Failure("Password reset failed.", result.Errors.Select(error => error.Description));
            }

            await _userManager.UpdateSecurityStampAsync(user);
            await _refreshTokenService.RevokeAllAsync(user.Id, cancellationToken);
            await _auditLogger.LogAsync("Auth.ResetPassword", user.Id, cancellationToken: cancellationToken);
            return ApiResult.Success("Password reset successfully.");
        }

        private static string DecodeResetToken(string token)
        {
            try
            {
                return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch (FormatException)
            {
                return token.Contains('%') ? WebUtility.UrlDecode(token) : token;
            }
        }
    }
}
