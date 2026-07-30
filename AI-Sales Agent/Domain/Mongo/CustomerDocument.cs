using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class CustomerDocument : MongoBaseDocument
    {
        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("organization_id")]
        public string OrganizationId { get; set; } = string.Empty;

        [BsonElement("external_id")]
        public string? ExternalId { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("first_name")]
        public string? FirstName { get; set; }

        [BsonElement("last_name")]
        public string? LastName { get; set; }

        [BsonElement("phone")]
        public string? Phone { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("notes")]
        public string? Notes { get; set; }

        [BsonElement("accepts_marketing")]
        public bool AcceptsMarketing { get; set; }

        [BsonElement("audit")]
        public AuditInfoModel Audit { get; set; } = new();

        [BsonElement("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
