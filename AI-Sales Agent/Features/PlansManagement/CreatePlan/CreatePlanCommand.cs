using MediatR;
using System;
using System.Collections.Generic;

namespace AI_Sales_Agent.Features.Plans.CreatePlan;

public record CreatePlanCommand(
    string PlanName,
    string PlanDescription,
    string PlanStatus,
    decimal PlanPrice,
    List<Guid> FeatureIds) : IRequest<Guid>;