using MediatR;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Data;
using System;

namespace AI_Sales_Agent.Features.FeaturesManagement.CreateFeature;

public class CreateFeatureHandler : IRequestHandler<CreateFeatureCommand, Guid>
{
    private readonly ApplicationDbContext _context;

    public CreateFeatureHandler(ApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateFeatureCommand request, CancellationToken cancellationToken)
    {
        var feature = new Feature
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Enabled = request.Enabled
        };

        _context.Features.Add(feature);
        await _context.SaveChangesAsync(cancellationToken);

        return feature.Id;
    }
}