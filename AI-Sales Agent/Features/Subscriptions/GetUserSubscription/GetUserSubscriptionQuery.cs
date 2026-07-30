using MediatR;
using AI_Sales_Agent.Features.Subscriptions.SubscribeToPlan;

namespace AI_Sales_Agent.Features.Subscriptions.GetUserSubscription;

public record GetUserSubscriptionQuery(Guid UserId) : IRequest<SubscriptionResponseDto?>;