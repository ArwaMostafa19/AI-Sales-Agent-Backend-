using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class KnowledgeVersionDocument : MongoBaseDocument
    {
        [BsonElement("organization_id")]
        public string OrganizationId { get; set; } = string.Empty;

        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("version_number")]
        public int VersionNumber { get; set; }

        [BsonElement("previous_version")]
        public int PreviousVersion { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "active";

        [BsonElement("document_count")]
        public int DocumentCount { get; set; }

        [BsonElement("chunk_count")]
        public int ChunkCount { get; set; }

        [BsonElement("files_processed")]
        public int FilesProcessed { get; set; }

        [BsonElement("files_skipped")]
        public int FilesSkipped { get; set; }

        [BsonElement("total_files")]
        public int TotalFiles { get; set; }

        [BsonElement("summary_generated")]
        public bool SummaryGenerated { get; set; }

        [BsonElement("embeddings_generated")]
        public bool EmbeddingsGenerated { get; set; }

        [BsonElement("vectors_synced")]
        public bool VectorsSynced { get; set; }

        [BsonElement("started_at")]
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [BsonElement("error")]
        public string? Error { get; set; }

        [BsonElement("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
