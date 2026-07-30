using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class DashboardInsightDocument : MongoBaseDocument
    {
        [BsonElement("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [BsonElement("recommendations")]
        public List<string> Recommendations { get; set; } = new();

        [BsonElement("total_revenue")]
        public double TotalRevenue { get; set; } = 0.0;

        [BsonElement("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();

        [BsonElement("calculated_at")]
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }
}
