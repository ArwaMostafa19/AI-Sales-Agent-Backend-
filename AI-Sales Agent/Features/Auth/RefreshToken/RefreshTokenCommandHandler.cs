using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using AI_Sales_Agent.Domain;

namespace AI_Sales_Agent.Features.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly UserManager<User> _userManager;
        private readonly IAuditLogger _auditLogger;

        public RefreshTokenCommandHandler(
            IRefreshTokenService refreshTokenService,
            IJwtTokenService jwtTokenService,
            UserManager<User> userManager,
            IAuditLogger auditLogger)
        {
            _refreshTokenService = refreshTokenService;
            _jwtTokenService = jwtTokenService;
            _userManager = userManager;
            _auditLogger = auditLogger;
        }

        public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenService.FindActiveAsync(request.RefreshToken, cancellationToken);
            if (refreshToken is null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var user = refreshToken.User;
            if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var newRefreshToken = await _refreshTokenService.RotateAsync(refreshToken, cancellationToken);
            await _auditLogger.LogAsync("Auth.RefreshToken", user.Id, cancellationToken: cancellationToken);

            var authResult = await _jwtTokenService.CreateTokenAsync(user, cancellationToken);
            return authResult with { RefreshToken = newRefreshToken };
        }
    }
}
