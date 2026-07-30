using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Features.Auth.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IAuditLogger _auditLogger;

        public LogoutCommandHandler(
            ICurrentUserService currentUserService,
            UserManager<User> userManager,
            IRefreshTokenService refreshTokenService,
            IAuditLogger auditLogger)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
            _refreshTokenService = refreshTokenService;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is not { } userId)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            await _refreshTokenService.RevokeAllAsync(user.Id, cancellationToken);
            await _userManager.UpdateSecurityStampAsync(user);
            await _auditLogger.LogAsync("Auth.Logout", user.Id, cancellationToken: cancellationToken);
            return ApiResult.Success("Logged out successfully.");
        }
    }
}
