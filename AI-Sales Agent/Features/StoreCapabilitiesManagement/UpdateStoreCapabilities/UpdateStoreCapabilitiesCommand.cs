using MediatR;

namespace AI_Sales_Agent.Features.StoreCapabilitiesManagement.UpdateStoreCapabilities;

public record UpdateStoreCapabilitiesCommand(
    string StoreId,
    bool HasPromoCode
) : IRequest<bool>;