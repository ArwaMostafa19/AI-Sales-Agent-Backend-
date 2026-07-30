using MediatR;

namespace AI_Sales_Agent.Features.Stores.GetStores
{
    public record GetStoresQuery : IRequest<IReadOnlyList<StoreResponse>>;
}
