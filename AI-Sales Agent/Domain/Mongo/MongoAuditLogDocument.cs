using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class MongoAuditLogDocument : MongoBaseDocument
    {
        [BsonElement("action")]
        public string Action { get; set; } = string.Empty;

        [BsonElement("actor_id")]
        public string? ActorId { get; set; }

        [BsonElement("actor_type")]
        public string ActorType { get; set; } = "user";

        [BsonElement("resource_type")]
        public string ResourceType { get; set; } = string.Empty;

        [BsonElement("resource_id")]
        public string? ResourceId { get; set; }

        [BsonElement("tenant_id")]
        public string? TenantId { get; set; }

        [BsonElement("details")]
        public Dictionary<string, object> Details { get; set; } = new();

        [BsonElement("ip_address")]
        public string? IpAddress { get; set; }

        [BsonElement("user_agent")]
        public string? UserAgent { get; set; }

        [BsonElement("outcome")]
        public string Outcome { get; set; } = "success";

        [BsonElement("failure_reason")]
        public string? FailureReason { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
