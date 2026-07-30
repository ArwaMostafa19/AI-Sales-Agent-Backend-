using FluentValidation;

namespace AI_Sales_Agent.Features.SellerProductsManagement.DeleteProduct;

public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required.");
    }
}