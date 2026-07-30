using MediatR;

namespace AI_Sales_Agent.Features.SellerProductsManagement.DeleteProduct;

public record DeleteProductCommand(
    string ProductId,
    string StoreId,
    bool SoftDelete = true
) : IRequest<bool>;