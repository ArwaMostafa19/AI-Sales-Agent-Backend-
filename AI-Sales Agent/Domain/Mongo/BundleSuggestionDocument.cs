using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class BundleSuggestionDocument : MongoBaseDocument
    {
        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("product_ids")]
        public List<string> ProductIds { get; set; } = new();

        [BsonElement("total_price")]
        public double TotalPrice { get; set; }

        [BsonElement("discount_percentage")]
        public double DiscountPercentage { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "active";
    }
}
