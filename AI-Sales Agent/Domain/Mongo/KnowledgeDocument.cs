using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class DocumentMetadataModel
    {
        [BsonElement("source_type")]
        public string SourceType { get; set; } = "manual";

        [BsonElement("source_uri")]
        public string? SourceUri { get; set; }

        [BsonElement("mime_type")]
        public string? MimeType { get; set; }

        [BsonElement("language")]
        public string Language { get; set; } = "en";

        [BsonElement("category")]
        public string? Category { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("attributes")]
        public Dictionary<string, object> Attributes { get; set; } = new();
    }

    public class DocumentVersionModel
    {
        [BsonElement("version_number")]
        public int VersionNumber { get; set; }

        [BsonElement("checksum")]
        public string? Checksum { get; set; }

        [BsonElement("created_by")]
        public string? CreatedBy { get; set; }

        [BsonElement("notes")]
        public string? Notes { get; set; }

        [BsonElement("is_current")]
        public bool IsCurrent { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class KnowledgeDocumentModel : MongoBaseDocument
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

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("source_url")]
        public string? SourceUrl { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "draft";

        [BsonElement("language")]
        public string Language { get; set; } = "en";

        [BsonElement("metadata")]
        public DocumentMetadataModel Metadata { get; set; } = new();

        [BsonElement("versions")]
        public List<DocumentVersionModel> Versions { get; set; } = new();

        [BsonElement("current_version")]
        public int CurrentVersion { get; set; } = 1;

        [BsonElement("chunking_strategy")]
        public string ChunkingStrategy { get; set; } = "manual";

        [BsonElement("processed_text")]
        public string? ProcessedText { get; set; }

        [BsonElement("page_count")]
        public int? PageCount { get; set; }

        [BsonElement("word_count")]
        public int? WordCount { get; set; }

        [BsonElement("char_count")]
        public int? CharCount { get; set; }

        [BsonElement("estimated_tokens")]
        public int? EstimatedTokens { get; set; }
    }
}
