using AI_Sales_Agent.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Plans.GetPlanDevelopmentPrice;

public class GetPlanDevelopmentPriceHandler : IRequestHandler<GetPlanDevelopmentPriceQuery, PlanDevelopmentPriceResponseDto?>
{
    private readonly ApplicationDbContext _context;

    public GetPlanDevelopmentPriceHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PlanDevelopmentPriceResponseDto?> Handle(GetPlanDevelopmentPriceQuery request, CancellationToken cancellationToken)
    {
        var plan = await _context.Plans
            .AsNoTracking()
            .Where(p => p.Id == request.PlanId && p.DeletedAt == null)
            .Select(p => new PlanDevelopmentPriceResponseDto(
                p.DevelopmentPrice
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return plan;
    }
}