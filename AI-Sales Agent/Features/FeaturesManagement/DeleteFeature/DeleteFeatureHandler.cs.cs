using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Data;
using System;

namespace AI_Sales_Agent.Features.FeaturesManagement.DeleteFeature;

public class DeleteFeatureHandler : IRequestHandler<DeleteFeatureCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public DeleteFeatureHandler(ApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteFeatureCommand request, CancellationToken cancellationToken)
    {
        var feature = await _context.Features
            .Include(f => f.PlanFeatures)
            .FirstOrDefaultAsync(f => f.Id == request.Id && f.DeletedAt == null, cancellationToken);

        if (feature == null) return false;

        var currentUtcTime = DateTime.UtcNow;
        feature.DeletedAt = currentUtcTime; // Soft Delete Feature globally

        // Soft Delete all active join records containing this feature across all subscription plans
        foreach (var planFeature in feature.PlanFeatures)
        {
            planFeature.DeletedAt = currentUtcTime;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}