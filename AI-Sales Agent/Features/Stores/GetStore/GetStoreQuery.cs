using MediatR;

namespace AI_Sales_Agent.Features.Stores.GetStore
{
    public record GetStoreQuery(Guid StoreId) : IRequest<StoreResponse?>;
}
