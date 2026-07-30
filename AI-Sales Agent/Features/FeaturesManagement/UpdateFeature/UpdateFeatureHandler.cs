using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Data;
using System;

namespace AI_Sales_Agent.Features.FeaturesManagement.UpdateFeature;

public class UpdateFeatureHandler : IRequestHandler<UpdateFeatureCommand, bool>
{
    private readonly ApplicationDbContext _context;

    public UpdateFeatureHandler(ApplicationDbContext context) => _context = context;

    public async Task<bool> Handle(UpdateFeatureCommand request, CancellationToken cancellationToken)
    {
        var feature = await _context.Features
            .FirstOrDefaultAsync(f => f.Id == request.Id && f.DeletedAt == null, cancellationToken);

        if (feature == null) return false;

        feature.Name = request.Name;
        feature.Description = request.Description;
        feature.Enabled = request.Enabled;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}