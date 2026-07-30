using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class KnowledgeJobDocument : MongoBaseDocument
    {
        [BsonElement("job_type")]
        public string JobType { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = "pending";

        [BsonElement("progress")]
        public double Progress { get; set; }

        [BsonElement("payload")]
        public Dictionary<string, object> Payload { get; set; } = new();

        [BsonElement("result")]
        public Dictionary<string, object>? Result { get; set; }

        [BsonElement("error_message")]
        public string? ErrorMessage { get; set; }

        [BsonElement("retry_count")]
        public int RetryCount { get; set; }

        [BsonElement("max_retries")]
        public int MaxRetries { get; set; } = 3;

        [BsonElement("store_id")]
        public string? StoreId { get; set; }

        [BsonElement("organization_id")]
        public string? OrganizationId { get; set; }

        [BsonElement("triggered_by")]
        public string? TriggeredBy { get; set; }

        [BsonElement("celery_task_id")]
        public string? CeleryTaskId { get; set; }

        [BsonElement("started_at")]
        public DateTime? StartedAt { get; set; }

        [BsonElement("completed_at")]
        public DateTime? CompletedAt { get; set; }
    }
}
