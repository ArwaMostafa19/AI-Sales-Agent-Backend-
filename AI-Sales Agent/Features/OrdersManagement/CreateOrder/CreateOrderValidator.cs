using FluentValidation;

namespace AI_Sales_Agent.Features.OrdersManagement.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required.");

        //RuleFor(x => x.OrgId)
        //    .NotEmpty().WithMessage("Organization ID is required.");

        RuleFor(x => x.LineItems)
            .NotEmpty().WithMessage("Order must contain at least one line item.");

        RuleFor(x => x.TotalPriceAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total price cannot be negative.");
    }
}