using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class AbandonedCartCampaignDocument : MongoBaseDocument
    {
        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("customer_id")]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("cart_details")]
        public Dictionary<string, object> CartDetails { get; set; } = new();

        [BsonElement("status")]
        public string Status { get; set; } = "pending";

        [BsonElement("recommneded_discount")]
        public string RecommendedDiscount { get; set; } = string.Empty;

        [BsonElement("maximum_allowed_discount")]
        public double MaximumAllowedDiscount { get; set; }

        [BsonElement("coupon_offered")]
        public string? CouponOffered { get; set; }
    }
}
