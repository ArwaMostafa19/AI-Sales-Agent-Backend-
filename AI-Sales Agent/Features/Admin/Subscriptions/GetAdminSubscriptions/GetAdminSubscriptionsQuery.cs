using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Subscriptions.GetAdminSubscriptions;

public record GetAdminSubscriptionsQuery(
    string? Status,
    int Page,
    int PageSize) : IRequest<AdminSubscriptionsResponse>;
