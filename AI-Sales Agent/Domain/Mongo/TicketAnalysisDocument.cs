using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class TicketAnalysisDocument : MongoBaseDocument
    {
        [BsonElement("ticket_id")]
        public string TicketId { get; set; } = string.Empty;

        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("customer_id")]
        public string CustomerId { get; set; } = string.Empty;

        [BsonElement("sentiment")]
        public string Sentiment { get; set; } = string.Empty;

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("summary")]
        public string Summary { get; set; } = string.Empty;

        [BsonElement("priority")]
        public string Priority { get; set; } = string.Empty;

        [BsonElement("suggested_response")]
        public string SuggestedResponse { get; set; } = string.Empty;

        [BsonElement("analyzed_at")]
        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    }
}
