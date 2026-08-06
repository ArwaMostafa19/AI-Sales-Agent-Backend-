using AI_Sales_Agent.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Subscriptions.HasUsedFreeTrial;

public class HasUsedFreeTrialHandler
    : IRequestHandler<HasUsedFreeTrialQuery, HasUsedFreeTrialResponse>
{
    private readonly ApplicationDbContext _context;

    public HasUsedFreeTrialHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HasUsedFreeTrialResponse> Handle(
        HasUsedFreeTrialQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Id == request.UserId,
                cancellationToken);

        if (user == null)
            throw new BadHttpRequestException("User not found.");

        return new HasUsedFreeTrialResponse(user.HasUsedTrial);
    }
}