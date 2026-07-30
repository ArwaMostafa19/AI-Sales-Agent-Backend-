using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class ConversationDocument : MongoBaseDocument
    {
        [BsonElement("customer_id")]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = "active";

        [BsonElement("messages")]
        public List<MessageDocument>? Messages { get; set; }
    }
}
