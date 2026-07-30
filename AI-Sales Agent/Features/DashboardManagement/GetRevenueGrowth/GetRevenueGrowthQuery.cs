using MediatR;

namespace AI_Sales_Agent.Features.DashboardManagement.GetRevenueGrowth;

public record GetRevenueGrowthQuery(Guid StoreId) : IRequest<RevenueGrowthResponseDto>;

public record RevenueGrowthResponseDto(
    double GrowthPercentage,
    string Status
);