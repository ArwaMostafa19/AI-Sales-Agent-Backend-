using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;

namespace AI_Sales_Agent.Features.Subscriptions.SubscribeToPlan;

public class SubscribeToPlanHandler : IRequestHandler<SubscribeToPlanCommand, SubscriptionResponseDto>
{
    private readonly ApplicationDbContext _context;

    public SubscribeToPlanHandler(ApplicationDbContext context) => _context = context;

    public async Task<SubscriptionResponseDto> Handle(SubscribeToPlanCommand request, CancellationToken cancellationToken)
    {
        // 1. التأكد من وجود البلان وأنها متاحة للبيع
        var plan = await _context.Plans
            .FirstOrDefaultAsync(p => p.Id == request.PlanId && p.DeletedAt == null && p.PlanStatus == "Active", cancellationToken);

        if (plan == null)
        {
            throw new BadHttpRequestException("The selected plan is invalid, inactive, or soft-deleted.");
        }

        // 2. البحث عن اشتراك السيلير الحالي
        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == request.UserId && s.DeletedAt == null, cancellationToken);

        if (existingSubscription != null)
        {
            // 🔄 دمج سيناريو التعديل والتنقل بين الخطط (Update / Change Plan)
            existingSubscription.PlanId = plan.Id;
            existingSubscription.Status = "Active";
            existingSubscription.RenewalDate = DateTime.UtcNow.AddMonths(1);
            existingSubscription.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // 🆕 سيناريو الاشتراك لأول مرة (Create New Subscription)
            existingSubscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PlanId = plan.Id,
                Status = "Active",
                RenewalDate = DateTime.UtcNow.AddMonths(1),
                CreatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(existingSubscription);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new SubscriptionResponseDto(
            existingSubscription.Id,
            existingSubscription.Status,
            existingSubscription.RenewalDate,
            plan.Id,
            plan.PlanName,
            plan.PlanPrice
        );
    }
}