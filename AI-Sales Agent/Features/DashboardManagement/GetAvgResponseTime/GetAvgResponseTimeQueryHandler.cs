using MediatR;
using MongoDB.Driver;
using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Services;

namespace AI_Sales_Agent.Features.DashboardManagement.GetAvgResponseTime;

public class GetConversationAnalyticsQueryHandler : IRequestHandler<GetConversationAnalyticsQuery, ConversationAnalyticsResponseDto>
{
    private readonly IMongoCollection<ConversationDocument> _conversationCollection;
    private readonly IMongoCollection<MessageDocument> _messageCollection;
    private readonly IDashboardNotifier _notifier;

    public GetConversationAnalyticsQueryHandler(IMongoDatabase mongoDatabase, IDashboardNotifier notifier)
    {
        _conversationCollection = mongoDatabase.GetCollection<ConversationDocument>("conversations");
        _messageCollection = mongoDatabase.GetCollection<MessageDocument>("messages");
        _notifier = notifier;
    }

    public async Task<ConversationAnalyticsResponseDto> Handle(GetConversationAnalyticsQuery request, CancellationToken cancellationToken)
    {
        string storeIdStr = request.StoreId.ToString();

        var storeFilter = Builders<ConversationDocument>.Filter.Eq("store_id", storeIdStr);

        var total = await _conversationCollection.CountDocumentsAsync(storeFilter, cancellationToken: cancellationToken);

        var activeFilter = Builders<ConversationDocument>.Filter.And(
            storeFilter,
            Builders<ConversationDocument>.Filter.Eq("status", "active")
        );
        var active = await _conversationCollection.CountDocumentsAsync(activeFilter, cancellationToken: cancellationToken);

        var resolvedFilter = Builders<ConversationDocument>.Filter.And(
            storeFilter,
            Builders<ConversationDocument>.Filter.Eq("status", "ended")
        );
        var resolved = await _conversationCollection.CountDocumentsAsync(resolvedFilter, cancellationToken: cancellationToken);

       
        var conversationIds = await _conversationCollection
            .Find(storeFilter)
            .Project(c => c.Id)
            .ToListAsync(cancellationToken);

        double avgResponseTime = 0.0;

        if (conversationIds.Any())
        {
            var messagesFilter = Builders<MessageDocument>.Filter.In("conversation_id", conversationIds);
            var messages = await _messageCollection
                .Find(messagesFilter)
                .SortBy(m => m.Timestamp)
                .ToListAsync(cancellationToken);

            var responseTimes = new List<double>();
            var groupedMessages = messages.GroupBy(m => m.ConversationId);

            foreach (var group in groupedMessages)
            {
                MessageDocument? lastUserMessage = null;

                foreach (var msg in group)
                {
                    if (msg.Role == "user")
                    {
                        lastUserMessage = msg;
                    }
                    else if (msg.Role == "assistant" && lastUserMessage != null)
                    {
                        var diffInSeconds = (msg.Timestamp - lastUserMessage.Timestamp).TotalSeconds;
                        if (diffInSeconds >= 0)
                        {
                            responseTimes.Add(diffInSeconds);
                        }
                        lastUserMessage = null;
                    }
                }
            }

            if (responseTimes.Any())
            {
                avgResponseTime = Math.Round(responseTimes.Average(), 2);
            }
        }

        var response = new ConversationAnalyticsResponseDto(
            Total: total,
            Active: active,
            Resolved: resolved,
            AvgResponseTime: avgResponseTime,
            Trend: new List<TrendDto>()
        );


        //await _notifier.NotifyAvgResponseTimeUpdatedAsync(request.StoreId, response);

        return response;
    }
}