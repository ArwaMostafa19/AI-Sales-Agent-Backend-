using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Sales_Agent.Features.Plans.GetAllPlans;

public class GetAllPlansHandler : IRequestHandler<GetAllPlansQuery, List<PlanListResponseDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAllPlansHandler(ApplicationDbContext context) => _context = context;

    public async Task<List<PlanListResponseDto>> Handle(GetAllPlansQuery request, CancellationToken cancellationToken)
    {
        return await _context.Plans
            .Where(p => p.DeletedAt == null)
            .Select(p => new PlanListResponseDto(
                p.Id,
                p.PlanName,
                p.PlanDescription,
                p.PlanStatus,
                p.PlanPrice,
                p.PlanFeatures
                    .Where(pf => pf.DeletedAt == null && pf.Feature != null && pf.Feature.DeletedAt == null)
                    .Select(pf => new PlanFeatureResponseDto(
                        pf.FeatureId,
                        pf.Feature.Name ?? "Unknown Feature",
                        pf.Feature.Description ?? "",
                        pf.Feature.Enabled ?? false
                    )).ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}