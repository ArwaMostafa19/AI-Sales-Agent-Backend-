using MediatR;
using MongoDB.Driver;
using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Services;

namespace AI_Sales_Agent.Features.DashboardManagement.GetResolutionRate;

public class GetResolutionRateQueryHandler : IRequestHandler<GetResolutionRateQuery, ResolutionRateResponseDto>
{
    private readonly IMongoCollection<ConversationDocument> _conversationsCollection;
    private readonly IMongoCollection<DashboardInsightDocument> _dashboardCollection;
    private readonly IDashboardNotifier _notifier;

    public GetResolutionRateQueryHandler(IMongoDatabase mongoDatabase, IDashboardNotifier notifier)
    {
        _conversationsCollection = mongoDatabase.GetCollection<ConversationDocument>("conversations");
        _dashboardCollection = mongoDatabase.GetCollection<DashboardInsightDocument>("DashboardInsights");
        _notifier = notifier;
    }

    public async Task<ResolutionRateResponseDto> Handle(GetResolutionRateQuery request, CancellationToken cancellationToken)
    {
        string storeIdStr = request.StoreId.ToString();

        var totalFilter = Builders<ConversationDocument>.Filter.Eq("store_id", storeIdStr);
        long totalConversations = await _conversationsCollection.CountDocumentsAsync(totalFilter, cancellationToken: cancellationToken);


        var resolvedFilter = Builders<ConversationDocument>.Filter.And(
            Builders<ConversationDocument>.Filter.Eq("store_id", storeIdStr),
            Builders<ConversationDocument>.Filter.Eq("status", "resolved")
        );
        long resolvedConversations = await _conversationsCollection.CountDocumentsAsync(resolvedFilter, cancellationToken: cancellationToken);

       
        double resolutionRate = 0.0;
        if (totalConversations > 0)
        {
            resolutionRate = (double)resolvedConversations / totalConversations * 100;
            resolutionRate = Math.Round(resolutionRate, 2);
        }

      
        var updateFilter = Builders<DashboardInsightDocument>.Filter.Eq("store_id", request.StoreId);
        var updateDefinition = Builders<DashboardInsightDocument>.Update
            .Set("store_id", request.StoreId)
            .Set("resolution_rate", resolutionRate)
            .Set("updated_at", DateTime.UtcNow);

        await _dashboardCollection.UpdateOneAsync(
            updateFilter,
            updateDefinition,
            new UpdateOptions { IsUpsert = true },
            cancellationToken);

        var response = new ResolutionRateResponseDto(
            ResolutionRate: resolutionRate,
            TotalConversations: totalConversations,
            ResolvedConversations: resolvedConversations
        );

        //await _notifier.NotifyResolutionRateUpdatedAsync(request.StoreId, response);

        return response;
    }
}