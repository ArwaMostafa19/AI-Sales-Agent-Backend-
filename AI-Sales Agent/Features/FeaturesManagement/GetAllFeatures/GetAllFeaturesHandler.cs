using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Data;
using System;

namespace AI_Sales_Agent.Features.FeaturesManagement.GetAllFeatures;

public class GetAllFeaturesHandler : IRequestHandler<GetAllFeaturesQuery, List<FeatureListResponseDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAllFeaturesHandler(ApplicationDbContext context) => _context = context;

    public async Task<List<FeatureListResponseDto>> Handle(GetAllFeaturesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Features
            .Where(f => f.DeletedAt == null)
            .Select(f => new FeatureListResponseDto(
                f.Id,
                f.Name,
                f.Description,
                f.Enabled ?? false
            ))
            .ToListAsync(cancellationToken);
    }
}