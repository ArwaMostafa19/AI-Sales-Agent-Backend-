using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class BusinessSummaryDocument : MongoBaseDocument
    {
        [BsonElement("organization_id")]
        public string OrganizationId { get; set; } = string.Empty;

        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("merchant_id")]
        public string MerchantId { get; set; } = string.Empty;

        [BsonElement("knowledge_version")]
        public int KnowledgeVersion { get; set; } = 1;

        [BsonElement("processing_status")]
        public string ProcessingStatus { get; set; } = "pending";

        [BsonElement("embedding_status")]
        public string EmbeddingStatus { get; set; } = "pending";

        [BsonElement("summary_status")]
        public string SummaryStatus { get; set; } = "pending";

        [BsonElement("checksum")]
        public string Checksum { get; set; } = string.Empty;

        [BsonElement("document_version")]
        public int DocumentVersion { get; set; } = 1;

        [BsonElement("source_type")]
        public string SourceType { get; set; } = "manual";

        [BsonElement("document_id")]
        public string DocumentId { get; set; } = string.Empty;

        [BsonElement("version_number")]
        public int VersionNumber { get; set; } = 1;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("summary")]
        public string Summary { get; set; } = string.Empty;

        [BsonElement("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
