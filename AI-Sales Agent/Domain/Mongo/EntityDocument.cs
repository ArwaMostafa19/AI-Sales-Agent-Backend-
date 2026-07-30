using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class EntityDocument : MongoBaseDocument
    {
        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("organization_id")]
        public string OrganizationId { get; set; } = string.Empty;

        [BsonElement("entity_type")]
        public string EntityType { get; set; } = string.Empty;

        [BsonElement("external_id")]
        public string ExternalId { get; set; } = string.Empty;

        [BsonElement("data")]
        public Dictionary<string, object> Data { get; set; } = new();

        [BsonElement("connection_id")]
        public string? ConnectionId { get; set; }

        [BsonElement("synced_at")]
        public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
    }
}
