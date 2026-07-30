using AI_Sales_Agent.Domain.Mongo;
using AI_Sales_Agent.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace AI_Sales_Agent.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.Seller)]
    [Route("api/stores/{storeId:guid}/conversations")]
    public class ConversationsController : ControllerBase
    {
        private readonly IMongoDatabase _mongoDatabase;

        public ConversationsController(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        [HttpGet]
        public async Task<IActionResult> GetConversations(Guid storeId)
        {
            var collection = _mongoDatabase.GetCollection<ConversationDocument>("conversations");
            var filter = Builders<ConversationDocument>.Filter.And(
                Builders<ConversationDocument>.Filter.Eq("store_id", storeId.ToString().ToLower()),
                Builders<ConversationDocument>.Filter.Eq("deleted_at", BsonNull.Value)
            );

            // Fetch conversations
            var conversations = await collection.Find(filter).ToListAsync();
            return Ok(conversations);
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(Guid storeId, string conversationId)
        {
            var collection = _mongoDatabase.GetCollection<MessageDocument>("messages");
            var filter = Builders<MessageDocument>.Filter.And(
                Builders<MessageDocument>.Filter.Eq("conversation_id", conversationId),
                Builders<MessageDocument>.Filter.Eq("deleted_at", BsonNull.Value)
            );

            var messages = await collection.Find(filter).Sort(Builders<MessageDocument>.Sort.Ascending("timestamp")).ToListAsync();
            return Ok(messages);
        }
    }
}
