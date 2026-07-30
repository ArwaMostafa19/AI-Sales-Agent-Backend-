namespace AI_Sales_Agent.Services
{
    public interface IDashboardNotifier
    {
        Task NotifyTotalRevenueUpdatedAsync(string storeId, double totalRevenue);
        Task NotifyRevenueGrowthUpdatedAsync(string storeId, object revenueGrowthData);
        Task NotifyResolutionRateUpdatedAsync(Guid storeId, object resolutionRateData);
        Task NotifyAvgResponseTimeUpdatedAsync(Guid storeId, object analyticsData);
        //Task NotifyRevenueGrowthUpdatedAsync(Guid storeGuid, RevenueGrowthResponseDto growthResponse);
    }
}