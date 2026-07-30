using MediatR;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.UpdateCategory;

public record UpdateCategoryCommand(
    string CategoryId,
    string StoreId,
    string Name,
    string? Description,
    string? ParentId,
    string? ImageUrl,
    int SortOrder
) : IRequest<bool>;