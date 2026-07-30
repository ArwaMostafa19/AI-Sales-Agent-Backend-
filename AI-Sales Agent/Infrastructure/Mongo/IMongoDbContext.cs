using AI_Sales_Agent.Domain.Mongo;
using MongoDB.Driver;

namespace AI_Sales_Agent.Infrastructure.Mongo
{
    public interface IMongoDbContext
    {
        IMongoDatabase Database { get; }

        IMongoCollection<ConversationDocument> Conversations { get; }
        IMongoCollection<MessageDocument> Messages { get; }
        IMongoCollection<KnowledgeDocumentModel> KnowledgeDocuments { get; }
        IMongoCollection<KnowledgeChunkDocument> KnowledgeChunks { get; }
        IMongoCollection<BusinessSummaryDocument> KnowledgeBusinessSummaries { get; }
        IMongoCollection<UploadMetadataDocument> KnowledgeUploads { get; }
        IMongoCollection<KnowledgeVersionDocument> KnowledgeVersions { get; }
        IMongoCollection<KnowledgeJobDocument> KnowledgeJobs { get; }
        IMongoCollection<AIRuntimeLogDocument> RuntimeLogs { get; }
        IMongoCollection<PromptHistoryDocument> PromptHistory { get; }
        IMongoCollection<RecommendationDocument> Recommendations { get; }
        IMongoCollection<BundleSuggestionDocument> BundleSuggestions { get; }
        IMongoCollection<AbandonedCartCampaignDocument> AbandonedCartCampaigns { get; }
        IMongoCollection<DashboardInsightDocument> DashboardInsights { get; }
        IMongoCollection<TicketAnalysisDocument> TicketAnalysis { get; }
        IMongoCollection<ProductDocument> Products { get; }
        IMongoCollection<CategoryDocument> Categories { get; }
        IMongoCollection<OrderDocument> Orders { get; }
        IMongoCollection<InventoryDocument> Inventory { get; }
        IMongoCollection<CustomerDocument> Customers { get; }
        IMongoCollection<IntegrationConnectionDocument> IntegrationConnections { get; }
        IMongoCollection<EntityDocument> Entities { get; }
        IMongoCollection<ApiKeyDocument> ApiKeys { get; }
        IMongoCollection<MongoAuditLogDocument> AuditLogs { get; }

        IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName);
    }
}
