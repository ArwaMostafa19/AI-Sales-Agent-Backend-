using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Analytics.GetAdminAiAnalytics;

public record GetAdminAiAnalyticsQuery : IRequest<AdminAiAnalyticsResponse>;
