using MediatR;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.GetAllCategories;

public record CategoryResponseDto(
    string Id,
    string StoreId,
    string OrgId,
    string Name,
    string? Description,
    string? Handle,
    string? ParentId,
    string? ImageUrl,
    int SortOrder,
    int ProductCount
);

public record GetAllCategoriesQuery(string StoreId, string? Search = null) : IRequest<List<CategoryResponseDto>>;