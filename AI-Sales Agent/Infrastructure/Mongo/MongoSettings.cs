namespace AI_Sales_Agent.Infrastructure.Mongo
{
    public class MongoSettings
    {
        public const string SectionName = "MongoSettings";

        public string ConnectionString { get; set; } = "mongodb://localhost:27017";

        public string DatabaseName { get; set; } = "ai_commerce";

        public int MinConnectionPoolSize { get; set; } = 10;

        public int MaxConnectionPoolSize { get; set; } = 100;

        public string KnowledgeDocumentsCollection { get; set; } = "knowledge_documents";

        public string KnowledgeChunksCollection { get; set; } = "knowledge_chunks";

        public string KnowledgeSummariesCollection { get; set; } = "knowledge_business_summaries";

        public string KnowledgeUploadsCollection { get; set; } = "knowledge_uploads";
    }
}
