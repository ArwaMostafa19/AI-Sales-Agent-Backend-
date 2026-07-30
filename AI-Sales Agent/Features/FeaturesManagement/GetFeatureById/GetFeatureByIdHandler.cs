using MediatR;
using Microsoft.EntityFrameworkCore;
using AI_Sales_Agent.Data;
using System;

namespace AI_Sales_Agent.Features.FeaturesManagement.GetFeatureById;

public class GetFeatureByIdHandler : IRequestHandler<GetFeatureByIdQuery, FeatureDetailsResponseDto?>
{
    private readonly ApplicationDbContext _context;

    public GetFeatureByIdHandler(ApplicationDbContext context) => _context = context;

    public async Task<FeatureDetailsResponseDto?> Handle(GetFeatureByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Features
            .Where(f => f.Id == request.Id && f.DeletedAt == null) // Ignore if soft-deleted
            .Select(f => new FeatureDetailsResponseDto(
                f.Id,
                f.Name,
                f.Description,
                f.Enabled ?? false
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}