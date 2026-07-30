using MediatR;

namespace AI_Sales_Agent.Features.DashboardManagement.GetTotalRevenue;

public record GetTotalRevenueQuery(string StoreId) : IRequest<double>;