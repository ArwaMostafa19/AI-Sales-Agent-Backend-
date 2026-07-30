using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class AIRuntimeLogDocument : MongoBaseDocument
    {
        [BsonElement("conversation_id")]
        public string ConversationId { get; set; } = string.Empty;

        [BsonElement("model")]
        public string Model { get; set; } = string.Empty;

        [BsonElement("prompt_tokens")]
        public string PromptTokens { get; set; } = string.Empty;

        [BsonElement("latency")]
        public double Latency { get; set; }

        [BsonElement("level")]
        public string Level { get; set; } = "INFO";

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("details")]
        public Dictionary<string, object> Details { get; set; } = new();

        [BsonElement("prompt_histories")]
        public List<Dictionary<string, object>>? PromptHistories { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
