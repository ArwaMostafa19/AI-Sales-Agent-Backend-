using AI_Sales_Agent.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Subscriptions.GetTrialStatus;

public class GetTrialStatusHandler
    : IRequestHandler<GetTrialStatusQuery, GetTrialStatusResponse>
{
    private readonly ApplicationDbContext _context;

    public GetTrialStatusHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetTrialStatusResponse> Handle(
        GetTrialStatusQuery request,
        CancellationToken cancellationToken)
    {
        //--------------------------------------------------------
        // التعديل:
        // نجيب آخر Subscription للمستخدم
        //--------------------------------------------------------

        var subscription = await _context.Subscriptions
            .Where(s =>
                s.UserId == request.UserId &&
                s.DeletedAt == null)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        //--------------------------------------------------------
        // المستخدم معندوش Subscription
        //--------------------------------------------------------

        if (subscription == null)
        {
            return new GetTrialStatusResponse
            {
                IsInTrial = false,
                RemainingDays = 0,
                TrialEndDate = null,
                Status = "NoSubscription"
            };
        }

        //--------------------------------------------------------
        // لو مش Trial
        //--------------------------------------------------------

        if (subscription.Status != "Trial")
        {
            return new GetTrialStatusResponse
            {
                IsInTrial = false,
                RemainingDays = 0,
                TrialEndDate = null,
                Status = subscription.Status
            };
        }

        //--------------------------------------------------------
        // نحسب الأيام المتبقية
        //--------------------------------------------------------

        var remainingDays = Math.Max(
                0,
                (subscription.TrialEndDate!.Value.Date - DateTime.UtcNow.Date).Days);

        //if (remainingDays < 0)
        //    remainingDays = 0;

        //--------------------------------------------------------
        // Response
        //--------------------------------------------------------

        return new GetTrialStatusResponse
        {
            IsInTrial = true,

            RemainingDays = remainingDays,

            TrialEndDate = subscription.TrialEndDate,

            Status = subscription.Status
        };
    }
}