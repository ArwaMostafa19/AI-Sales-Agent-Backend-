using MediatR;

namespace AI_Sales_Agent.Features.Stores.UpdateStoreSettings
{
    public record UpdateStoreSettingsCommand(
        Guid StoreId,
        string Currency,
        string Language,
        string Timezone) : IRequest<StoreResponse?>;
}
