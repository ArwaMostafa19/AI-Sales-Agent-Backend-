using AI_Sales_Agent.Domain.Mongo;
using MediatR;

namespace AI_Sales_Agent.Features.DashboardManagement.RefreshDashboardInsights;

public record RefreshDashboardInsightsCommand(OrderDocument Order) : IRequest;
