using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Data;
using AI_Sales_Agent.Features.Subscriptions.SubscribeToPlan;

namespace AI_Sales_Agent.Features.Subscriptions.GetUserSubscription;

public class GetUserSubscriptionHandler : IRequestHandler<GetUserSubscriptionQuery, SubscriptionResponseDto?>
{
    private readonly ApplicationDbContext _context;

    public GetUserSubscriptionHandler(ApplicationDbContext context) => _context = context;

    public async Task<SubscriptionResponseDto?> Handle(GetUserSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.UserId == request.UserId && s.DeletedAt == null && (s.Status == "Active" || s.Status == "Trial") , cancellationToken);

        if (subscription == null || subscription.Plan == null) return null;

        return new SubscriptionResponseDto(
            subscription.Id,
            subscription.Status,
            subscription.RenewalDate,
            subscription.Plan.Id,
            subscription.Plan.PlanName,
            subscription.Plan.PlanPrice
        );
    }
}