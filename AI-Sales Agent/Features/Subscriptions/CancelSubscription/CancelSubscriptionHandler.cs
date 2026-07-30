using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Data;

namespace AI_Sales_Agent.Features.Subscriptions.CancelSubscription;

public class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public CancelSubscriptionHandler(ApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == request.UserId && s.DeletedAt == null, cancellationToken);

        if (subscription == null || subscription.Status == "Cancelled")
        {
            return false;
        }

        subscription.Status = "Cancelled";
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}