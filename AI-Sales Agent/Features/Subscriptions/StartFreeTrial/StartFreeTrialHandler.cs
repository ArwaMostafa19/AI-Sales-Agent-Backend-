using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Subscriptions.StartFreeTrial;

public class StartFreeTrialHandler
    : IRequestHandler<StartFreeTrialCommand, StartFreeTrialResponse>
{
    private readonly ApplicationDbContext _context;

    public StartFreeTrialHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StartFreeTrialResponse> Handle(
        StartFreeTrialCommand request,
        CancellationToken cancellationToken)
    {
        //----------------------------------------------------
        // نتأكد إن البلان موجودة ومفعلة
        //----------------------------------------------------

        var plan = await _context.Plans
            .FirstOrDefaultAsync(
                p => p.Id == request.PlanId &&
                     p.DeletedAt == null &&
                     p.PlanStatus == "Active",
                cancellationToken);

        if (plan == null)
            throw new BadHttpRequestException("Selected plan not found.");

        //----------------------------------------------------
        // نتأكد إن اليوزر موجود
        //----------------------------------------------------

        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.Id == request.UserId,
                cancellationToken);

        if (user == null)
            throw new BadHttpRequestException("User not found.");

        //----------------------------------------------------
        // لو استخدم الـ Trial قبل كده
        //----------------------------------------------------

        if (user.HasUsedTrial)
            throw new BadHttpRequestException("Free trial has already been used.");

        //----------------------------------------------------
        // نتأكد إنه معندوش Trial أو Active
        //----------------------------------------------------

        var currentSubscription = await _context.Subscriptions
            .AnyAsync(
                s =>
                    s.UserId == request.UserId &&
                    s.DeletedAt == null &&
                    (s.Status == "Trial" || s.Status == "Active"),
                cancellationToken);

        if (currentSubscription)
            throw new BadHttpRequestException(
                "You already have an active or trial subscription.");

        //----------------------------------------------------
        // بداية ونهاية الـ Trial
        //----------------------------------------------------

        var trialStart = DateTime.UtcNow;

       


        var trialEnd = trialStart.AddDays(plan.TrialDays);

        //var trialEnd = DateTime.UtcNow.AddMinutes(5);

        //----------------------------------------------------
        // إنشاء Subscription جديدة
        //----------------------------------------------------

        var subscription = new Subscription
        {
            UserId = request.UserId,

            PlanId = plan.Id,

            Status = "Trial",

            IsTrial = true,

            TrialStartDate = trialStart,

            TrialEndDate = trialEnd,

            RenewalDate = trialEnd
        };

        _context.Subscriptions.Add(subscription);

        //----------------------------------------------------
        // المستخدم استهلك الـ Trial
        //----------------------------------------------------

        user.HasUsedTrial = true;

        await _context.SaveChangesAsync(cancellationToken);

        //----------------------------------------------------
        // Response
        //----------------------------------------------------

        return new StartFreeTrialResponse
        {
            SubscriptionId = subscription.Id,

            TrialDays = plan.TrialDays,

            TrialEndDate = trialEnd,

            Message = "Free trial started successfully."
        };
    }
}