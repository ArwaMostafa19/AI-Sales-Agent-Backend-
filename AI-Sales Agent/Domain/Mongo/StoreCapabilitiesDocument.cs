using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AI_Sales_Agent.Domain.Mongo;

public class StoreCapabilitiesDocument : MongoBaseDocument
{
    [BsonElement("store_id")]
    public string StoreId { get; set; } = string.Empty;

    [BsonElement("capabilities")]
    public BsonDocument Capabilities { get; set; } = new BsonDocument();

    [BsonElement("auto_detected")]
    public BsonDocument? AutoDetected { get; set; }
}