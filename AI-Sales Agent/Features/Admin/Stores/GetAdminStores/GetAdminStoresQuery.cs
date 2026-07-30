using AI_Sales_Agent.Features.Admin.Shared;
using MediatR;

namespace AI_Sales_Agent.Features.Admin.Stores.GetAdminStores;

public record GetAdminStoresQuery(
    string? Search,
    string? Platform,
    string? Status,
    int Page,
    int PageSize) : IRequest<PagedResponse<AdminStoreResponse>>;
