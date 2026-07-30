using System.Security.Claims;

namespace AI_Sales_Agent.Infrastructure.Auth
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(value, out var userId) ? userId : null;
            }
        }
    }
}
