using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class CategoryDocument : MongoBaseDocument
    {
        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("org_id")]
        public string OrgId { get; set; } = string.Empty;

        [BsonElement("external_id")]
        public string? ExternalId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("handle")]
        public string? Handle { get; set; }

        [BsonElement("parent_id")]
        public string? ParentId { get; set; }

        [BsonElement("image_url")]
        public string? ImageUrl { get; set; }

        [BsonElement("sort_order")]
        public int SortOrder { get; set; }

        [BsonElement("product_count")]
        public int ProductCount { get; set; }

        [BsonElement("audit")]
        public AuditInfoModel Audit { get; set; } = new();
    }
}
