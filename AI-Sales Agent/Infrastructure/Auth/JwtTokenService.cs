using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AI_Sales_Agent.Infrastructure.Auth
{
    public interface IJwtTokenService
    {
        Task<AuthResult> CreateTokenAsync(User user, CancellationToken cancellationToken);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _options;
        private readonly UserManager<User> _userManager;

        public JwtTokenService(IOptions<JwtOptions> options, UserManager<User> userManager)
        {
            _options = options.Value;
            _userManager = userManager;
        }

        public async Task<AuthResult> CreateTokenAsync(User user, CancellationToken cancellationToken)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);
            var securityStamp = await _userManager.GetSecurityStampAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new("security_stamp", securityStamp),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(userClaims);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new AuthResult(
                new JwtSecurityTokenHandler().WriteToken(token),
                string.Empty,
                expiresAt,
                user.Id,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                user.EmailConfirmed);
        }
    }
}
