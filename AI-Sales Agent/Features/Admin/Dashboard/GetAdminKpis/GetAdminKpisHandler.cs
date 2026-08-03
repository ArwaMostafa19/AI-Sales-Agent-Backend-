using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Features.Admin.Shared;
using AI_Sales_Agent.Infrastructure.Auth;
using AI_Sales_Agent.Infrastructure.Mongo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.Admin.Dashboard.GetAdminKpis;

public class GetAdminKpisHandler : IRequestHandler<GetAdminKpisQuery, AdminKpiResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMongoDbContext _mongoDbContext;

    public GetAdminKpisHandler(ApplicationDbContext dbContext, IMongoDbContext mongoDbContext)
    {
        _dbContext = dbContext;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<AdminKpiResponse> Handle(GetAdminKpisQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);

        // Store KPIs
        var storesQuery = _dbContext.Stores.AsNoTracking().Where(store => store.DeletedAt == null);
        var totalStores = await storesQuery.CountAsync(cancellationToken);
        var activeStores = await storesQuery.CountAsync(store => store.Status == "Active", cancellationToken);
        var currentMonthStores = await storesQuery.CountAsync(store => store.CreatedAt >= currentMonthStart, cancellationToken);
        var previousMonthStores = await storesQuery.CountAsync(
            store => store.CreatedAt >= previousMonthStart && store.CreatedAt < currentMonthStart,
            cancellationToken);
        var storeGrowth = AdminDashboardQueryHelpers.GrowthPercent(currentMonthStores, previousMonthStores);

        // User KPIs
        var totalUsers = await _dbContext.Users.AsNoTracking().CountAsync(cancellationToken);
        var emailConfirmedUsers = await _dbContext.Users
            .AsNoTracking()
            .CountAsync(user => user.EmailConfirmed, cancellationToken);
        var sellerRoleId = await _dbContext.Roles
            .AsNoTracking()
            .Where(role => role.Name == Roles.Admin)
            .Select(role => role.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var totalSellers = sellerRoleId == Guid.Empty
            ? 0
            : await _dbContext.UserRoles
                .AsNoTracking()
                .CountAsync(userRole => userRole.RoleId == sellerRoleId, cancellationToken);

        // Revenue KPIs
        var activeSubscriptionsQuery = _dbContext.Subscriptions
            .AsNoTracking()
            .Where(subscription =>
                subscription.DeletedAt == null &&
                subscription.Status == "Active" &&
                subscription.Plan != null &&
                subscription.Plan.DeletedAt == null);

        var activeSubscriptions = await activeSubscriptionsQuery.CountAsync(cancellationToken);
        var monthlyRecurringRevenue = await activeSubscriptionsQuery
            .SumAsync(subscription => subscription.Plan == null ? 0 : subscription.Plan.PlanPrice, cancellationToken);

        // Conversation KPIs
        var deletedFilter = Builders<ConversationDocument>.Filter.Eq(conversation => conversation.DeletedAt, null);
        var totalConversations = await _mongoDbContext.Conversations.CountDocumentsAsync(deletedFilter, null, cancellationToken);

        var currentMonthFilter = deletedFilter & Builders<ConversationDocument>.Filter.Gte(conversation => conversation.CreatedAt, currentMonthStart);
        var previousMonthFilter = deletedFilter &
            Builders<ConversationDocument>.Filter.Gte(conversation => conversation.CreatedAt, previousMonthStart) &
            Builders<ConversationDocument>.Filter.Lt(conversation => conversation.CreatedAt, currentMonthStart);

        var currentMonthConversations = await _mongoDbContext.Conversations.CountDocumentsAsync(currentMonthFilter, null, cancellationToken);
        var previousMonthConversations = await _mongoDbContext.Conversations.CountDocumentsAsync(previousMonthFilter, null, cancellationToken);
        var conversationGrowth = AdminDashboardQueryHelpers.GrowthPercent(currentMonthConversations, previousMonthConversations);

        // AI Stats
        var aiStats = await AdminDashboardQueryHelpers.GetAiStats(_mongoDbContext, cancellationToken);

        return new AdminKpiResponse(
            totalStores,
            activeStores,
            storeGrowth,
            totalUsers,
            totalSellers,
            emailConfirmedUsers,
            totalConversations,
            conversationGrowth,
            activeSubscriptions,
            monthlyRecurringRevenue,
            aiStats.ConversionRate,
            aiStats.HighIntentMessages);
    }
}
