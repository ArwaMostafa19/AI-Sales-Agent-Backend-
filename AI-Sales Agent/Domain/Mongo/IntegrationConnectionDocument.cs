using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class IntegrationConnectionDocument : MongoBaseDocument
    {
        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("organization_id")]
        public string OrganizationId { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("platform_name")]
        public string PlatformName { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = "active";

        [BsonElement("credentials")]
        public Dictionary<string, object> Credentials { get; set; } = new();

        [BsonElement("settings")]
        public Dictionary<string, object> Settings { get; set; } = new();

        [BsonElement("webhook_secret")]
        public string? WebhookSecret { get; set; }

        [BsonElement("last_sync_at")]
        public DateTime? LastSyncAt { get; set; }

        [BsonElement("sync_status")]
        public string? SyncStatus { get; set; }

        [BsonElement("pagination")]
        public PaginationModel Pagination { get; set; } = new();
    }
}
