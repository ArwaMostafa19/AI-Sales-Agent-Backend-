using System.Security.Cryptography;
using System.Text;
using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AI_Sales_Agent.Infrastructure.Auth
{
    public interface IRefreshTokenService
    {
        Task<string> CreateAsync(User user, CancellationToken cancellationToken);
        Task<RefreshToken?> FindActiveAsync(string token, CancellationToken cancellationToken);
        Task<string> RotateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken);
        string Hash(string token);
    }

    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtOptions _options;

        public RefreshTokenService(
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IOptions<JwtOptions> options)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _options = options.Value;
        }

        public async Task<string> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var token = GenerateToken();
            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = Hash(token),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays),
                CreatedByIp = GetIpAddress()
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
            return token;
        }

        public async Task<RefreshToken?> FindActiveAsync(string token, CancellationToken cancellationToken)
        {
            var tokenHash = Hash(token);
            var refreshToken = await _dbContext.RefreshTokens
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.TokenHash == tokenHash, cancellationToken);

            return refreshToken?.IsActive == true ? refreshToken : null;
        }

        public async Task<string> RotateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            var newToken = GenerateToken();
            var newTokenHash = Hash(newToken);

            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = GetIpAddress();
            refreshToken.ReplacedByTokenHash = newTokenHash;

            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = newTokenHash,
                UserId = refreshToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays),
                CreatedByIp = GetIpAddress()
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
            return newToken;
        }

        public async Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken)
        {
            var activeTokens = await _dbContext.RefreshTokens
                .Where(token => token.UserId == userId && token.RevokedAt == null && token.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = GetIpAddress();
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public string Hash(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        private static string GenerateToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string? GetIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }
    }
}
