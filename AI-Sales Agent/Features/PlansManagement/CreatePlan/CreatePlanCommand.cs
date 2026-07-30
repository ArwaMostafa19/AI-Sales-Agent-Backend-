using MediatR;
using System;
using System.Collections.Generic;

namespace AI_Sales_Agent.Features.Plans.CreatePlan;

public record CreatePlanCommand(
    string PlanName,
    string PlanDescription,
    string PlanStatus,
    decimal PlanPrice,
    long NumOfTokens,
    List<string> AiModels,
    List<Guid> FeatureIds) : IRequest<Guid>;