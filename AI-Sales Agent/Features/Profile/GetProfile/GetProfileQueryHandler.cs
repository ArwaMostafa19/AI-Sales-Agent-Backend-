using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Profile.GetProfile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileResponseDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _context;

        public GetProfileQueryHandler(ICurrentUserService currentUserService, ApplicationDbContext context)
        {
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<ProfileResponseDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is not { } userId)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var user = await _context.Users
                .Include(u => u.Stores.Where(s => s.DeletedAt == null))
                .Include(u => u.Subscription)
                    .ThenInclude(s => s!.Plan)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var storesDto = user.Stores.Select(s => new ProfileStoreDto(
                s.Id,
                s.Name,
                s.Description,
                s.Platform,
                s.ShopDomain,
                s.Currency,
                s.Language,
                s.Timezone,
                s.Status
            )).ToList();

            ProfileSubscriptionDto? subscriptionDto = null;
            if (user.Subscription != null && user.Subscription.Plan != null && user.Subscription.DeletedAt == null)
            {
                subscriptionDto = new ProfileSubscriptionDto(
                    user.Subscription.Id,
                    user.Subscription.Status,
                    user.Subscription.RenewalDate,
                    user.Subscription.Plan.Id,
                    user.Subscription.Plan.PlanName,
                    user.Subscription.Plan.PlanPrice
                );
            }

            return new ProfileResponseDto(
                user.Id,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.ProfilePictureUrl,
                user.LastLogin,
                storesDto,
                subscriptionDto
            );
        }
    }
}
