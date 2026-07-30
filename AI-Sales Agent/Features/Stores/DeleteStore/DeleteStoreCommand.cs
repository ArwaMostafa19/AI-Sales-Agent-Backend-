using AI_Sales_Agent.Abstractions;
using MediatR;

namespace AI_Sales_Agent.Features.Stores.DeleteStore
{
    public record DeleteStoreCommand(Guid StoreId) : IRequest<ApiResult>;
}
