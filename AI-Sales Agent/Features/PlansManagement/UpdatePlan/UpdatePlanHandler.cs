using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Data;
using System;

namespace AI_Sales_Agent.Features.Plans.UpdatePlan;

public class UpdatePlanHandler : IRequestHandler<UpdatePlanCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public UpdatePlanHandler(ApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        
        var activeFeaturesCount = await _context.Features
            .Where(f => request.FeatureIds.Contains(f.Id)
                        && f.DeletedAt == null
                        && f.Enabled == true)
            .CountAsync(cancellationToken);

        if (activeFeaturesCount != request.FeatureIds.Count)
        {
            throw new BadHttpRequestException("Cannot update plan. One or more selected features are currently disabled or invalid.");
        }

       
        var plan = await _context.Plans
            .Include(p => p.PlanFeatures)
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.DeletedAt == null, cancellationToken);

        if (plan == null) return false;

        plan.PlanName = request.PlanName;
        plan.PlanDescription = request.PlanDescription;
        plan.PlanStatus = request.PlanStatus;
        plan.PlanPrice = request.PlanPrice;

        _context.PlanFeatures.RemoveRange(plan.PlanFeatures);

        plan.PlanFeatures = request.FeatureIds.Select(fId => new PlanFeature
        {
            PlanId = plan.Id,
            FeatureId = fId
        }).ToList();

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}