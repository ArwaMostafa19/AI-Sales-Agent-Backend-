using AI_Sales_Agent.Data;
using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Admin.Subscriptions.GetAdminSubscriptions;

public class GetAdminSubscriptionsHandler : IRequestHandler<GetAdminSubscriptionsQuery, AdminSubscriptionsResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetAdminSubscriptionsHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AdminSubscriptionsResponse> Handle(
        GetAdminSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var subscriptionsQuery = _dbContext.Subscriptions
            .AsNoTracking()
            .Where(subscription => subscription.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            subscriptionsQuery = subscriptionsQuery.Where(subscription => subscription.Status == request.Status);
        }

        var total = await subscriptionsQuery.CountAsync(cancellationToken);
        var subscriptions = await subscriptionsQuery
            .OrderByDescending(subscription => subscription.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(subscription => new AdminSubscriptionResponse(
                subscription.Id,
                subscription.Status,
                subscription.RenewalDate,
                subscription.CreatedAt,
                subscription.UserId,
                subscription.User == null ? null : subscription.User.Email,
                subscription.PlanId,
                subscription.Plan == null ? null : subscription.Plan.PlanName,
                subscription.Plan == null ? 0 : subscription.Plan.PlanPrice))
            .ToListAsync(cancellationToken);

        var plans = await _dbContext.Plans
            .AsNoTracking()
            .Where(plan => plan.DeletedAt == null)
            .OrderBy(plan => plan.PlanPrice)
            .Select(plan => new AdminPlanResponse(
                plan.Id,
                plan.PlanName,
                plan.PlanDescription,
                plan.PlanStatus,
                plan.PlanPrice,
                plan.Subscriptions.Count(subscription =>
                    subscription.DeletedAt == null &&
                    subscription.Status == "Active")))
            .ToListAsync(cancellationToken);

        var activeSubscriptionsQuery = _dbContext.Subscriptions
            .AsNoTracking()
            .Where(subscription =>
                subscription.DeletedAt == null &&
                subscription.Status == "Active" &&
                subscription.Plan != null &&
                subscription.Plan.DeletedAt == null);

        var activeSubscriptions = await activeSubscriptionsQuery.CountAsync(cancellationToken);
        var mrr = await activeSubscriptionsQuery
            .SumAsync(subscription => subscription.Plan == null ? 0 : subscription.Plan.PlanPrice, cancellationToken);

        return new AdminSubscriptionsResponse(
            new AdminSubscriptionSummaryResponse(activeSubscriptions, mrr, plans.Count),
            new PagedResponse<AdminSubscriptionResponse>(subscriptions, page, pageSize, total),
            plans);
    }
}
