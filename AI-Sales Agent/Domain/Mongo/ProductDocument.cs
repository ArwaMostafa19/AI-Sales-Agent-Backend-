using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class ProductDocument : MongoBaseDocument
    {
        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("organization_id")]
        public string? OrganizationId { get; set; }

        [BsonElement("external_id")]
        public string? ExternalId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("handle")]
        public string? Handle { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "draft";

        [BsonElement("product_type")]
        public string? ProductType { get; set; }

        [BsonElement("vendor")]
        public string? Vendor { get; set; }

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("images")]
        public List<ImageModel> Images { get; set; } = new();

        [BsonElement("variants")]
        public List<VariantModel> Variants { get; set; } = new();

        [BsonElement("options")]
        public List<ProductOptionModel> Options { get; set; } = new();

        [BsonElement("seo")]
        public SEOModel Seo { get; set; } = new();

        [BsonElement("category_id")]
        public string? CategoryId { get; set; }

        [BsonElement("max_allowed_discount")]
        public double MaxAllowedDiscount { get; set; }

        [BsonElement("audit")]
        public AuditInfoModel Audit { get; set; } = new();

        [BsonElement("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
