using AI_Sales_Agent.Infrastructure.Mongo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace AI_Sales_Agent.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/health/mongo")]
    public class MongoHealthController : ControllerBase
    {
        private readonly IMongoDbContext _mongoDbContext;
        private readonly ILogger<MongoHealthController> _logger;

        public MongoHealthController(
            IMongoDbContext mongoDbContext,
            ILogger<MongoHealthController> logger)
        {
            _mongoDbContext = mongoDbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Check(CancellationToken cancellationToken)
        {
            try
            {
                var ping = await _mongoDbContext.Database.RunCommandAsync<BsonDocument>(
                    new BsonDocument("ping", 1),
                    cancellationToken: cancellationToken);

                return Ok(new
                {
                    status = "connected",
                    database = _mongoDbContext.Database.DatabaseNamespace.DatabaseName,
                    ok = ping.GetValue("ok", 0).ToDouble(),
                    checkedAt = DateTime.UtcNow
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "MongoDB health check failed.");

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    status = "disconnected",
                    message = "MongoDB connection failed.",
                    checkedAt = DateTime.UtcNow
                });
            }
        }
    }
}
