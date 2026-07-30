using MediatR;

namespace AI_Sales_Agent.Features.DashboardManagement.GetAvgResponseTime;

public record GetConversationAnalyticsQuery(Guid StoreId) : IRequest<ConversationAnalyticsResponseDto>;

public record ConversationAnalyticsResponseDto(
    long Total,
    long Active,
    long Resolved,
    double AvgResponseTime,
    List<TrendDto> Trend
);

public record TrendDto(
    string Date,
    long Total
);