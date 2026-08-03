using AI_Sales_Agent.Features.Admin.Analytics.GetAdminAiAnalytics;
using AI_Sales_Agent.Features.Admin.AuditLogs.GetAdminAuditLogs;
using AI_Sales_Agent.Features.Admin.Dashboard.GetAdminKpis;
using AI_Sales_Agent.Features.Admin.Dashboard.GetAdminOverview;
using AI_Sales_Agent.Features.Admin.Dashboard.GetAdminPlatformDistribution;
using AI_Sales_Agent.Features.Admin.Dashboard.GetAdminRecentStores;
using AI_Sales_Agent.Features.Admin.Dashboard.GetAdminRevenueGrowth;
using AI_Sales_Agent.Features.Admin.Dashboard.GetAdminSentimentBreakdown;
using AI_Sales_Agent.Features.Admin.Dashboard.GetAdminSystemHealth;
using AI_Sales_Agent.Features.Admin.Dashboard.GetAdminTopIntents;
using AI_Sales_Agent.Features.Admin.Shared;
using AI_Sales_Agent.Features.Admin.Stores.GetAdminStores;
using AI_Sales_Agent.Features.Admin.Stores.UpdateStoreStatus;
using AI_Sales_Agent.Features.Admin.Subscriptions.GetAdminSubscriptions;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sales_Agent.Features.Admin;

[ApiController]
[Authorize(Roles = Roles.SuperAdmin)]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private const int DefaultPageSize = 20;
    private readonly ISender _sender;

    public AdminController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// 1. KPI Cards (Total Active Stores, AI Conversations, MRR, AI Conversion Rate)
    /// </summary>
    [HttpGet("dashboard/kpis")]
    public async Task<ActionResult<AdminKpiResponse>> GetKpis(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetAdminKpisQuery(), cancellationToken));
    }

    /// <summary>
    /// 2. Revenue & Growth Line Chart
    /// </summary>
    [HttpGet("dashboard/revenue-growth")]
    public async Task<ActionResult<IReadOnlyList<AdminGrowthPointResponse>>> GetRevenueGrowth(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetAdminRevenueGrowthQuery(), cancellationToken));
    }

    /// <summary>
    /// 3. Store Platforms Donut Chart (Shopify, WooCommerce, Custom API)
    /// </summary>
    [HttpGet("dashboard/platform-distribution")]
    public async Task<ActionResult<IReadOnlyList<CountBreakdownItem>>> GetPlatformDistribution(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetAdminPlatformDistributionQuery(), cancellationToken));
    }

    /// <summary>
    /// 4. Top Customer Intents Progress Bars
    /// </summary>
    [HttpGet("dashboard/top-intents")]
    public async Task<ActionResult<IReadOnlyList<CountBreakdownItem>>> GetTopIntents(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetAdminTopIntentsQuery(), cancellationToken));
    }

    /// <summary>
    /// 5. Sentiment Breakdown (Positive, Neutral, Negative)
    /// </summary>
    [HttpGet("dashboard/sentiment-breakdown")]
    public async Task<ActionResult<IReadOnlyList<CountBreakdownItem>>> GetSentimentBreakdown(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetAdminSentimentBreakdownQuery(), cancellationToken));
    }

    /// <summary>
    /// 6. System Health Cards (API Latency, AI Service Online, DB Load)
    /// </summary>
    [HttpGet("dashboard/system-health")]
    public async Task<ActionResult<AdminMongoHealthResponse>> GetSystemHealth(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetAdminSystemHealthQuery(), cancellationToken));
    }

    /// <summary>
    /// 7. Recent Registered Stores Table Widget
    /// </summary>
    [HttpGet("dashboard/recent-stores")]
    public async Task<ActionResult<IReadOnlyList<AdminStoreResponse>>> GetRecentStores(
        [FromQuery] int count = 8,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _sender.Send(new GetAdminRecentStoresQuery(count), cancellationToken));
    }

    /// <summary>
    /// Single Overview endpoint returning all dashboard data at once (Optional)
    /// </summary>
    [HttpGet("dashboard/overview")]
    public async Task<ActionResult<AdminOverviewResponse>> GetOverview(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetAdminOverviewQuery(), cancellationToken));
    }

    /// <summary>
    /// Full Stores List with Pagination and Filtering ("View All Stores" button)
    /// </summary>
    [HttpGet("stores")]
    public async Task<ActionResult<PagedResponse<AdminStoreResponse>>> GetStores(
        [FromQuery] string? search,
        [FromQuery] string? platform,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminStoresQuery(search, platform, status, page, pageSize);
        return Ok(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Update Store Status (Activate / Suspend)
    /// </summary>
    [HttpPatch("stores/{storeId:guid}/status")]
    public async Task<IActionResult> UpdateStoreStatus(
        Guid storeId,
        UpdateStoreStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return BadRequest(new { message = "Status is required." });
        }

        var updated = await _sender.Send(
            new UpdateStoreStatusCommand(storeId, request.Status, request.Reason),
            cancellationToken);

        return updated ? NoContent() : NotFound(new { message = "Store not found." });
    }

    /// <summary>
    /// Admin Subscriptions Management
    /// </summary>
    [HttpGet("subscriptions")]
    public async Task<ActionResult<AdminSubscriptionsResponse>> GetSubscriptions(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminSubscriptionsQuery(status, page, pageSize);
        return Ok(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// AI Analytics
    /// </summary>
    [HttpGet("analytics/ai")]
    public async Task<ActionResult<AdminAiAnalyticsResponse>> GetAiAnalytics(CancellationToken cancellationToken)
    {
        return Ok(await _sender.Send(new GetAdminAiAnalyticsQuery(), cancellationToken));
    }

    /// <summary>
    /// Audit Logs
    /// </summary>
    [HttpGet("audit-logs")]
    public async Task<ActionResult<PagedResponse<AdminAuditLogResponse>>> GetAuditLogs(
        [FromQuery] string? action,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminAuditLogsQuery(action, page, pageSize);
        return Ok(await _sender.Send(query, cancellationToken));
    }
}
