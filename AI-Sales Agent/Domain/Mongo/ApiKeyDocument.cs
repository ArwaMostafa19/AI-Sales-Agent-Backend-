using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class ApiKeyDocument : MongoBaseDocument
    {
        [BsonElement("key_hash")]
        public string KeyHash { get; set; } = string.Empty;

        [BsonElement("key_prefix")]
        public string KeyPrefix { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("scopes")]
        public List<string> Scopes { get; set; } = new();

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;

        [BsonElement("expires_at")]
        public DateTime? ExpiresAt { get; set; }
    }
}
