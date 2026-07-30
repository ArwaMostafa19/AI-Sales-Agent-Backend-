using AI_Sales_Agent.Domain.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AI_Sales_Agent.Infrastructure.Mongo
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly MongoSettings _settings;

        public MongoDbContext(IMongoClient client, IOptions<MongoSettings> settings)
        {
            _settings = settings.Value;
            Database = client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoDatabase Database { get; }

        public IMongoCollection<ConversationDocument> Conversations => GetCollection<ConversationDocument>(MongoCollectionNames.Conversations);

        public IMongoCollection<MessageDocument> Messages => GetCollection<MessageDocument>(MongoCollectionNames.Messages);

        public IMongoCollection<KnowledgeDocumentModel> KnowledgeDocuments => GetCollection<KnowledgeDocumentModel>(_settings.KnowledgeDocumentsCollection);

        public IMongoCollection<KnowledgeChunkDocument> KnowledgeChunks => GetCollection<KnowledgeChunkDocument>(_settings.KnowledgeChunksCollection);

        public IMongoCollection<BusinessSummaryDocument> KnowledgeBusinessSummaries => GetCollection<BusinessSummaryDocument>(_settings.KnowledgeSummariesCollection);

        public IMongoCollection<UploadMetadataDocument> KnowledgeUploads => GetCollection<UploadMetadataDocument>(_settings.KnowledgeUploadsCollection);

        public IMongoCollection<KnowledgeVersionDocument> KnowledgeVersions => GetCollection<KnowledgeVersionDocument>(MongoCollectionNames.KnowledgeVersions);

        public IMongoCollection<KnowledgeJobDocument> KnowledgeJobs => GetCollection<KnowledgeJobDocument>(MongoCollectionNames.KnowledgeJobs);

        public IMongoCollection<AIRuntimeLogDocument> RuntimeLogs => GetCollection<AIRuntimeLogDocument>(MongoCollectionNames.RuntimeLogs);

        public IMongoCollection<PromptHistoryDocument> PromptHistory => GetCollection<PromptHistoryDocument>(MongoCollectionNames.PromptHistory);

        public IMongoCollection<RecommendationDocument> Recommendations => GetCollection<RecommendationDocument>(MongoCollectionNames.Recommendations);

        public IMongoCollection<BundleSuggestionDocument> BundleSuggestions => GetCollection<BundleSuggestionDocument>(MongoCollectionNames.BundleSuggestions);

        public IMongoCollection<AbandonedCartCampaignDocument> AbandonedCartCampaigns => GetCollection<AbandonedCartCampaignDocument>(MongoCollectionNames.AbandonedCartCampaigns);

        public IMongoCollection<DashboardInsightDocument> DashboardInsights => GetCollection<DashboardInsightDocument>(MongoCollectionNames.DashboardInsights);

        public IMongoCollection<TicketAnalysisDocument> TicketAnalysis => GetCollection<TicketAnalysisDocument>(MongoCollectionNames.TicketAnalysis);

        public IMongoCollection<ProductDocument> Products => GetCollection<ProductDocument>(MongoCollectionNames.Products);

        public IMongoCollection<CategoryDocument> Categories => GetCollection<CategoryDocument>(MongoCollectionNames.Categories);

        public IMongoCollection<OrderDocument> Orders => GetCollection<OrderDocument>(MongoCollectionNames.Orders);

        public IMongoCollection<InventoryDocument> Inventory => GetCollection<InventoryDocument>(MongoCollectionNames.Inventory);

        public IMongoCollection<CustomerDocument> Customers => GetCollection<CustomerDocument>(MongoCollectionNames.Customers);

        public IMongoCollection<IntegrationConnectionDocument> IntegrationConnections => GetCollection<IntegrationConnectionDocument>(MongoCollectionNames.IntegrationConnections);

        public IMongoCollection<EntityDocument> Entities => GetCollection<EntityDocument>(MongoCollectionNames.Entities);

        public IMongoCollection<ApiKeyDocument> ApiKeys => GetCollection<ApiKeyDocument>(MongoCollectionNames.ApiKeys);

        public IMongoCollection<MongoAuditLogDocument> AuditLogs => GetCollection<MongoAuditLogDocument>(MongoCollectionNames.AuditLogs);

        public IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            return Database.GetCollection<TDocument>(collectionName);
        }
    }
}
