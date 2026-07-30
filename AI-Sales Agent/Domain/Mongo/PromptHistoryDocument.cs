using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo
{
    public class PromptHistoryDocument : MongoBaseDocument
    {
        [BsonElement("runtimeId")]
        public string RuntimeId { get; set; } = string.Empty;

        [BsonElement("provider")]
        public string Provider { get; set; } = string.Empty;

        [BsonElement("context")]
        public string Context { get; set; } = string.Empty;

        [BsonElement("model")]
        public string Model { get; set; } = string.Empty;

        [BsonElement("system_prompt")]
        public string SystemPrompt { get; set; } = string.Empty;

        [BsonElement("user_prompt")]
        public string UserPrompt { get; set; } = string.Empty;

        [BsonElement("llm_response")]
        public string LlmResponse { get; set; } = string.Empty;

        [BsonElement("token_used")]
        public int TokenUsed { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
