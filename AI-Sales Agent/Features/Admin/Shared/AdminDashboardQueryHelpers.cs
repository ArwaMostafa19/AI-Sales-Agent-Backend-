using System.Diagnostics;
using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Mongo;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AI_Sales_Agent.Features.Admin.Shared;

internal static class AdminDashboardQueryHelpers
{
    private static readonly string[] HighIntentKeywords = ["buy", "purchase", "checkout", "order", "discount", "cart"];

    public static IQueryable<Store> BuildStoresQuery(
        ApplicationDbContext dbContext,
        string? search,
        string? platform,
        string? status)
    {
        var query = dbContext.Stores
            .AsNoTracking()
            .Where(store => store.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(store =>
                store.Name.Contains(search) ||
                store.ShopDomain.Contains(search) ||
                (store.User != null && store.User.Email != null && store.User.Email.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(platform))
        {
            query = query.Where(store => store.Platform == platform);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(store => store.Status == status);
        }

        return query;
    }

    public static IQueryable<AdminStoreResponse> SelectAdminStoreResponse(this IQueryable<Store> query)
    {
        return query.Select(store => new AdminStoreResponse(
            store.Id,
            store.Name,
            store.Platform,
            store.ShopDomain,
            store.Status,
            store.Currency,
            store.Language,
            store.Timezone,
            store.CreatedAt,
            store.UpdatedAt,
            store.UserId,
            store.User == null ? null : store.User.Email,
            store.User == null ? null : (store.User.FirstName + " " + store.User.LastName).Trim(),
            store.User != null && store.User.Subscription != null && store.User.Subscription.Plan != null
                ? store.User.Subscription.Plan.PlanName
                : null,
            store.User != null && store.User.Subscription != null ? store.User.Subscription.Status : null));
    }

    public static async Task<AdminAiStats> GetAiStats(
        IMongoDbContext mongoDbContext,
        CancellationToken cancellationToken)
    {
        var totalMessages = await mongoDbContext.Messages.CountDocumentsAsync(
            Builders<MessageDocument>.Filter.Eq(message => message.DeletedAt, null),
            null,
            cancellationToken);

        var topIntents = await GetMongoBreakdown(
            mongoDbContext.Messages,
            "intent",
            totalMessages,
            8,
            cancellationToken);

        var sentimentBreakdown = await GetMongoBreakdown(
            mongoDbContext.Messages,
            "sentiment",
            totalMessages,
            5,
            cancellationToken);

        var highIntentMessages = await CountHighIntentMessages(mongoDbContext, cancellationToken);
        var conversionRate = ToPercent(highIntentMessages, totalMessages);

        return new AdminAiStats(
            totalMessages,
            highIntentMessages,
            conversionRate,
            topIntents,
            sentimentBreakdown);
    }

    public static async Task<AdminProductStatsResponse> GetProductStats(
        IMongoDbContext mongoDbContext,
        CancellationToken cancellationToken)
    {
        var productFilter = Builders<ProductDocument>.Filter.Eq(product => product.DeletedAt, null);
        var categoryFilter = Builders<CategoryDocument>.Filter.Eq(category => category.DeletedAt, null);
        var productsCount = await mongoDbContext.Products.CountDocumentsAsync(productFilter, null, cancellationToken);
        var categoriesCount = await mongoDbContext.Categories.CountDocumentsAsync(categoryFilter, null, cancellationToken);

        var topCategories = await mongoDbContext.Categories.Aggregate()
            .Match(new BsonDocument { ["deleted_at"] = BsonNull.Value })
            .Sort(new BsonDocument("product_count", -1))
            .Limit(8)
            .Project(new BsonDocument
            {
                ["name"] = "$name",
                ["product_count"] = "$product_count"
            })
            .ToListAsync(cancellationToken);

        return new AdminProductStatsResponse(
            productsCount,
            categoriesCount,
            topCategories
                .Select(category =>
                {
                    var productCount = category.GetValue("product_count", 0).ToInt64();
                    var categoryName = category.GetValue("name", "Unknown").ToString() ?? "Unknown";

                    return new CountBreakdownItem(
                        categoryName,
                        productCount,
                        ToPercent(productCount, productsCount));
                })
                .ToList());
    }

    public static async Task<AdminMongoHealthResponse> GetMongoHealth(
        IMongoDbContext mongoDbContext,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await mongoDbContext.Database.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1),
                cancellationToken: cancellationToken);

            stopwatch.Stop();
            return new AdminMongoHealthResponse(
                "connected",
                mongoDbContext.Database.DatabaseNamespace.DatabaseName,
                stopwatch.ElapsedMilliseconds,
                result.GetValue("ok", 0).ToDouble(),
                DateTime.UtcNow);
        }
        catch
        {
            stopwatch.Stop();
            return new AdminMongoHealthResponse(
                "disconnected",
                mongoDbContext.Database.DatabaseNamespace.DatabaseName,
                stopwatch.ElapsedMilliseconds,
                0,
                DateTime.UtcNow);
        }
    }

    public static decimal GrowthPercent(long current, long previous)
    {
        if (previous == 0)
        {
            return current > 0 ? 100 : 0;
        }

        return Math.Round(((decimal)(current - previous) / previous) * 100, 2);
    }

    public static decimal ToPercent(long value, long total)
    {
        if (total <= 0)
        {
            return 0;
        }

        return Math.Round(((decimal)value / total) * 100, 2);
    }

    private static async Task<long> CountHighIntentMessages(
        IMongoDbContext mongoDbContext,
        CancellationToken cancellationToken)
    {
        var keywordFilters = HighIntentKeywords
            .Select(keyword => Builders<MessageDocument>.Filter.Regex("intent", new BsonRegularExpression(keyword, "i")))
            .ToArray();

        if (keywordFilters.Length == 0)
        {
            return 0;
        }

        var filter = Builders<MessageDocument>.Filter.Eq(message => message.DeletedAt, null) &
            Builders<MessageDocument>.Filter.Or(keywordFilters);

        return await mongoDbContext.Messages.CountDocumentsAsync(filter, null, cancellationToken);
    }

    private static async Task<List<CountBreakdownItem>> GetMongoBreakdown<TDocument>(
        IMongoCollection<TDocument> collection,
        string fieldName,
        long total,
        int limit,
        CancellationToken cancellationToken)
    {
        var pipeline = await collection.Aggregate()
            .Match(new BsonDocument
            {
                ["deleted_at"] = BsonNull.Value,
                [fieldName] = new BsonDocument("$nin", new BsonArray { BsonNull.Value, string.Empty })
            })
            .Group(new BsonDocument
            {
                ["_id"] = $"${fieldName}",
                ["count"] = new BsonDocument("$sum", 1)
            })
            .Sort(new BsonDocument("count", -1))
            .Limit(limit)
            .ToListAsync(cancellationToken);

        return pipeline
            .Select(document =>
            {
                var label = document.GetValue("_id", "Unknown").IsBsonNull
                    ? "Unknown"
                    : document.GetValue("_id", "Unknown").ToString() ?? "Unknown";
                var count = document.GetValue("count", 0).ToInt64();
                return new CountBreakdownItem(label, count, ToPercent(count, total));
            })
            .ToList();
    }
}
