using MediatR;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.CreateCategory;

public record CreateCategoryCommand(
    string StoreId,
    string? OrgId,
    string Name,
    string? Description,
    string? ParentId,
    string? ImageUrl,
    int SortOrder = 0
) : IRequest<string>;