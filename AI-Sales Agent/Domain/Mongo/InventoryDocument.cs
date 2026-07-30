using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class InventoryDocument : MongoBaseDocument
    {
        [BsonElement("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("variant_id")]
        public string VariantId { get; set; } = string.Empty;

        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("org_id")]
        public string OrgId { get; set; } = string.Empty;

        [BsonElement("external_id")]
        public string? ExternalId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("available")]
        public int Available { get; set; }

        [BsonElement("committed")]
        public int Committed { get; set; }

        [BsonElement("incoming")]
        public int Incoming { get; set; }

        [BsonElement("location_id")]
        public string? LocationId { get; set; }

        [BsonElement("location_name")]
        public string? LocationName { get; set; }

        [BsonElement("low_stock_threshold")]
        public int? LowStockThreshold { get; set; }

        [BsonElement("audit")]
        public AuditInfoModel Audit { get; set; } = new();
    }
}
