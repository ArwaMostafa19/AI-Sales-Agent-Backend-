using AI_Sales_Agent.Features.SellerCategoriesManagement.GetAllCategories;
using MediatR;

namespace AI_Sales_Agent.Features.SellerCategoriesManagement.GetCategoryById;

public record GetCategoryByIdQuery(string CategoryId, string StoreId) : IRequest<CategoryResponseDto?>;