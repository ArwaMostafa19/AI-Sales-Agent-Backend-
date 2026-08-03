using AI_Sales_Agent.Data;
using AI_Sales_Agent.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Subscriptions.CreateCheckoutSession;

public class CreateCheckoutSessionHandler
    : IRequestHandler<CreateCheckoutSessionCommand, CreateCheckoutSessionResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IStripeService _stripeService;

    public CreateCheckoutSessionHandler(
        ApplicationDbContext context,
        IStripeService stripeService)
    {
        _context = context;
        _stripeService = stripeService;
    }

    public async Task<CreateCheckoutSessionResponse> Handle(
        CreateCheckoutSessionCommand request,
        CancellationToken cancellationToken)
    {
        var plan = await _context.Plans
            .FirstOrDefaultAsync(
                p => p.Id == request.PlanId
                  && p.DeletedAt == null
                  && p.PlanStatus == "Active",
                cancellationToken);

        if (plan is null)
            throw new BadHttpRequestException("Selected plan not found.");

        //var user = await _context.Users
        //    .FirstOrDefaultAsync(
        //    u => u.Id == request.UserId,
        //    cancellationToken);

        //if (user == null)
        //    throw new BadHttpRequestException("User not found.");

        var hasCurrentSubscription = await _context.Subscriptions
            .AnyAsync(s =>
            s.UserId == request.UserId &&
            s.DeletedAt == null &&
            (s.Status == "Active"),
            cancellationToken);

        if (hasCurrentSubscription)
        {
            throw new BadHttpRequestException(
                "You already have an active subscription.");
        }


        var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(
            request.UserId,
            plan.Id,
            plan.PlanName,
            plan.PlanDescription,
            plan.PlanPrice
            );

        return new CreateCheckoutSessionResponse(checkoutUrl);
    }
}