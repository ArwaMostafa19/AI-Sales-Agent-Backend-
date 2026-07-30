using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminRecentStores;

public record GetAdminRecentStoresQuery(int Count = 8) : IRequest<IReadOnlyList<AdminStoreResponse>>;
