using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IAuditLogger _auditLogger;

        public LoginCommandHandler(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService,
            IRefreshTokenService refreshTokenService,
            IAuditLogger auditLogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _auditLogger = auditLogger;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new UnauthorizedAccessException("Please verify your email before login.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var refreshToken = await _refreshTokenService.CreateAsync(user, cancellationToken);
            await _auditLogger.LogAsync("Auth.Login", user.Id, cancellationToken: cancellationToken);

            var authResult = await _jwtTokenService.CreateTokenAsync(user, cancellationToken);
            return authResult with { RefreshToken = refreshToken };
        }
    }
}
