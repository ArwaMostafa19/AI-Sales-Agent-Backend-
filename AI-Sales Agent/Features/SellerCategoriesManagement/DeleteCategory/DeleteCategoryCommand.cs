using MediatR;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.DeleteCategory;

public record DeleteCategoryCommand(
    string CategoryId,
    string StoreId,
    bool SoftDelete = true
) : IRequest<bool>;