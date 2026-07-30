using MediatR;

namespace AI_Sales_Agent.Features.Plans.DeletePlan;

public record DeletePlanCommand(Guid Id) : IRequest<bool>;