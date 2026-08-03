using MediatR;

namespace AI_Sales_Agent.Features.SellerStoreManagement.GetStoreCapabilities;

public record GetStoreCapabilitiesQuery(
    string StoreId
) : IRequest<StoreCapabilitiesResponseDto?>;


public record StoreCapabilitiesResponseDto(
    string StoreId,
    Dictionary<string, bool> Capabilities
);