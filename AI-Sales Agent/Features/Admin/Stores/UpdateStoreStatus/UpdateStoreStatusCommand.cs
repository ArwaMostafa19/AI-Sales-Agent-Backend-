using MediatR;

namespace AI_Sales_Agent.Features.Admin.Stores.UpdateStoreStatus;

public record UpdateStoreStatusRequest(string Status, string? Reason);

public record UpdateStoreStatusCommand(
    Guid StoreId,
    string Status,
    string? Reason) : IRequest<bool>;
