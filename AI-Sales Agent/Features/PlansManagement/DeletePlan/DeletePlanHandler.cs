using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Data;
using System;

namespace AI_Sales_Agent.Features.Plans.DeletePlan;

public class DeletePlanHandler : IRequestHandler<DeletePlanCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public DeletePlanHandler(ApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _context.Plans
            .Include(p => p.PlanFeatures)
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.DeletedAt == null, cancellationToken);

        if (plan == null) return false;

        var currentUtcTime = DateTime.UtcNow;
        plan.DeletedAt = currentUtcTime;

       
        foreach (var planFeature in plan.PlanFeatures)
        {
            planFeature.DeletedAt = currentUtcTime;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}