using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Data;
using System;

namespace AI_Sales_Agent.Features.Plans.GetPlanById;

public class GetPlanByIdHandler : IRequestHandler<GetPlanByIdQuery, PlanDetailsResponseDto?>
{
    private readonly ApplicationDbContext _context;

    public GetPlanByIdHandler(ApplicationDbContext context) => _context = context;

    public async Task<PlanDetailsResponseDto?> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Plans
            .Where(p => p.Id == request.Id && p.DeletedAt == null)
            .Select(p => new PlanDetailsResponseDto(
                p.Id,
                p.PlanName,
                p.PlanDescription,
                p.PlanStatus,
                p.PlanPrice,
                p.PlanFeatures
                    .Where(pf => pf.DeletedAt == null && pf.Feature.DeletedAt == null)
                    .Select(pf => new PlanFeatureDetailsDto(
                        pf.FeatureId,
                        pf.Feature.Name,
                        pf.Feature.Description,
                        pf.Feature.Enabled ?? false
                    )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}