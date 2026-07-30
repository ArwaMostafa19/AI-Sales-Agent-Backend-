using AI_Sales_Agent.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AI_Sales_Agent.Services;

public class DashboardNotifier : IDashboardNotifier
{
    private readonly IHubContext<DashboardHub> _hubContext;

    public DashboardNotifier(IHubContext<DashboardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    
    public async Task NotifyTotalRevenueUpdatedAsync(string storeId, double totalRevenue)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveTotalRevenue", new { storeId, totalRevenue });
    }

    public async Task NotifyRevenueGrowthUpdatedAsync(string storeId, object revenueGrowthData)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveRevenueGrowth", revenueGrowthData);
    }

    public async Task NotifyResolutionRateUpdatedAsync(Guid storeId, object resolutionRateData)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveResolutionRate", resolutionRateData);
    }

    public async Task NotifyAvgResponseTimeUpdatedAsync(Guid storeId, object analyticsData)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveConversationAnalytics", analyticsData);
    }
}