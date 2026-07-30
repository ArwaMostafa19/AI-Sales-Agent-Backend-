using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class RecommendationDocument : MongoBaseDocument
    {
        [BsonElement("conversation_id")]
        public string ConversationId { get; set; } = string.Empty;

        [BsonElement("customer_id")]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("recommended_product_ids")]
        public List<string> RecommendedProductIds { get; set; } = new();

        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("accepted")]
        public bool Accepted { get; set; }

        [BsonElement("rationale")]
        public string Rationale { get; set; } = string.Empty;
    }
}
