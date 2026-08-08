using MediatR;
using Stripe;
using System;
using System.Collections.Generic;

namespace AI_Sales_Agent.Features.Plans.CreatePlan;

public record CreatePlanCommand(
    string PlanName,
    string PlanDescription,
    string PlanStatus,
    decimal PlanPrice,
    long NumOfTokens,
    int TrialDays,
    decimal Developmentprice,
    List<string> AiModels,
    List<Guid> FeatureIds) : IRequest<Guid>;