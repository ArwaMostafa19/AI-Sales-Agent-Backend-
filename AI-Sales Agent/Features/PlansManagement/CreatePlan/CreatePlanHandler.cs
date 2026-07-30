using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Sales_Agent.Features.Plans.CreatePlan;

public class CreatePlanHandler : IRequestHandler<CreatePlanCommand, Guid>
{
    private readonly ApplicationDbContext _context;

    public CreatePlanHandler(ApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {

        var uniqueFeatureIds = request.FeatureIds.Distinct().ToList();

        var activeFeaturesCount = await _context.Features
            .Where(f => uniqueFeatureIds.Contains(f.Id)
                        && f.DeletedAt == null
                        && f.Enabled == true)
            .CountAsync(cancellationToken);

        if (activeFeaturesCount != uniqueFeatureIds.Count)
        {
            throw new BadHttpRequestException("Cannot add features to the plan. One or more selected features are currently disabled or do not exist.");
        }

        var plan = new Plan
        {
            Id = Guid.NewGuid(),
            PlanName = request.PlanName,
            PlanDescription = request.PlanDescription,
            PlanStatus = request.PlanStatus,
            PlanPrice = request.PlanPrice,
            PlanFeatures = uniqueFeatureIds.Select(fId => new PlanFeature
            {
                FeatureId = fId
            }).ToList()
        };

        _context.Plans.Add(plan);
        await _context.SaveChangesAsync(cancellationToken);

        return plan.Id;
    }
}