using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _dbContext;

        public JwtTokenService(
            IOptions<JwtOptions> options,
            UserManager<User> userManager,
            ApplicationDbContext dbContext)
        {
            _options = options.Value;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<AuthResult> CreateTokenAsync(User user, CancellationToken cancellationToken)
        {
            var issuedAt = DateTime.UtcNow;
            var expiresAt = issuedAt.AddMinutes(_options.ExpirationMinutes);
            var securityStamp = await _userManager.GetSecurityStampAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            var storeId = await _dbContext.Stores
                .AsNoTracking()
                .Where(s => s.UserId == user.Id && s.DeletedAt == null)
                .Select(s => (Guid?)s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var organizationId = await _dbContext.Organizations
                .AsNoTracking()
                .Where(o => o.UserId == user.Id && o.DeletedAt == null)
                .Select(o => (Guid?)o.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new("security_stamp", securityStamp),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (storeId.HasValue)
            {
                claims.Add(new Claim("store_id", storeId.Value.ToString()));
            }

            if (organizationId.HasValue)
            {
                claims.Add(new Claim("org_id", organizationId.Value.ToString()));
            }

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
                issuedAt,
                expiresAt,
                user.Id,
                storeId,
                organizationId,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                user.EmailConfirmed);
        }
    }
}
