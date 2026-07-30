using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;

namespace AI_Sales_Agent.Infrastructure.Audit
{
    public interface IAuditLogger
    {
        Task LogAsync(string action, Guid? userId = null, string? metadata = null, CancellationToken cancellationToken = default);
    }

    public class AuditLogger : IAuditLogger
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogger(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, Guid? userId = null, string? metadata = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                _dbContext.AuditLogs.Add(new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = httpContext?.Request.Headers.UserAgent.ToString(),
                    Metadata = metadata
                });

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                // Audit failure should not fail the main request.
            }
        }
    }
}
