using MediatR;

namespace AI_Sales_Agent.Features.Stores.CreateStore
{
    public record CreateStoreCommand(
        string Name,
        string Description,
        string Platform,
        string ShopDomain,
        string Currency,
        string Language,
        string Timezone) : IRequest<StoreResponse>;
}
