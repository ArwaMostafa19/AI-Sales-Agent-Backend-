using MediatR;

namespace AI_Sales_Agent.Features.Subscriptions.HasUsedFreeTrial;

public record HasUsedFreeTrialQuery(Guid UserId)
    : IRequest<HasUsedFreeTrialResponse>;

public record HasUsedFreeTrialResponse(bool HasUsedFreeTrial);