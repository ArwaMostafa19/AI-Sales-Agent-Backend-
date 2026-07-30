using MediatR;
using System;
using System.Collections.Generic;

namespace AI_Sales_Agent.Features.Plans.GetAllPlans;

public record GetAllPlansQuery() : IRequest<List<PlanListResponseDto>>;

public record PlanListResponseDto(
    Guid Id,
    string PlanName,
    string PlanDescription,
    string PlanStatus,
    decimal PlanPrice,
    List<PlanFeatureResponseDto> Features);

public record PlanFeatureResponseDto(
    Guid FeatureId,
    string FeatureName,
    string FeatureDescription,
    bool Enabled);