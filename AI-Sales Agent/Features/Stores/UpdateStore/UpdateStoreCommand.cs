using MediatR;

namespace AI_Sales_Agent.Features.Stores.UpdateStore
{
    public record UpdateStoreCommand(
        Guid StoreId,
        string Name,
        string Description,
        string Platform,
        string ShopDomain,
        string Status) : IRequest<StoreResponse?>;
}
