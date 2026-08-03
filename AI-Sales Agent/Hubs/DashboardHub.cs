using Microsoft.AspNetCore.SignalR;

namespace AI_Sales_Agent.Hubs;

public class DashboardHub : Hub
{
    private const string StoreIdItemKey = "DashboardStoreId";

    public static string GetStoreGroupName(string storeId) => $"dashboard-store:{storeId}";

    public Task JoinStore(string storeId) => JoinStoreGroup(storeId);

    public async Task JoinStoreGroup(string storeId)
    {
        if (string.IsNullOrWhiteSpace(storeId))
        {
            throw new HubException("A store ID is required.");
        }

        if (Context.Items.TryGetValue(StoreIdItemKey, out var currentStoreId)
            && currentStoreId is string previousStoreId
            && !string.Equals(previousStoreId, storeId, StringComparison.Ordinal))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetStoreGroupName(previousStoreId));
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GetStoreGroupName(storeId));
        Context.Items[StoreIdItemKey] = storeId;
    }
}
