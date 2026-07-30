using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class UploadMetadataDocument : MongoBaseDocument
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

        [BsonElement("original_filename")]
        public string OriginalFilename { get; set; } = string.Empty;

        [BsonElement("stored_filename")]
        public string StoredFilename { get; set; } = string.Empty;

        [BsonElement("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [BsonElement("file_size")]
        public long FileSize { get; set; }

        [BsonElement("mime_type")]
        public string MimeType { get; set; } = string.Empty;

        [BsonElement("extension")]
        public string Extension { get; set; } = string.Empty;

        [BsonElement("content_type")]
        public string ContentType { get; set; } = "document";

        [BsonElement("uploaded_by")]
        public string UploadedBy { get; set; } = string.Empty;

        [BsonElement("knowledge_scope")]
        public string KnowledgeScope { get; set; } = "general";

        [BsonElement("status")]
        public string Status { get; set; } = "uploaded";

        [BsonElement("document_metadata")]
        public Dictionary<string, object> DocumentMetadata { get; set; } = new();

        [BsonElement("virus_scan_status")]
        public string VirusScanStatus { get; set; } = "pending";
    }
}
