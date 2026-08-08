using MediatR;
using System;

namespace AI_Sales_Agent.Features.Plans.GetPlanDevelopmentPrice;

public record GetPlanDevelopmentPriceQuery(
    Guid PlanId
) : IRequest<PlanDevelopmentPriceResponseDto?>;

public record PlanDevelopmentPriceResponseDto(
    decimal DevelopmentPrice
);