using AI_Sales_Agent.Data;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Domain.Mongo;
using MongoDB.Bson;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.Results;
using AI_Sales_Agent.Infrastructure.Mongo;

namespace AI_Sales_Agent.Features.Stores.CreateStore
{
    public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, StoreResponse>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly MongoDbContext _mongoDbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditLogger _auditLogger;

        public CreateStoreCommandHandler(
            ApplicationDbContext dbContext,
            MongoDbContext mongoDbContext,
            ICurrentUserService currentUserService,
            IAuditLogger auditLogger)
        {
            _dbContext = dbContext;
            _mongoDbContext = mongoDbContext;
            _currentUserService = currentUserService;
            _auditLogger = auditLogger;
        }

        public async Task<StoreResponse> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is not { } userId)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var hasValidSubscription = await _dbContext.Subscriptions
                .AnyAsync(s =>
                    s.UserId == userId &&
                    s.DeletedAt == null &&
                    (s.Status == "Active" ||
                     s.Status == "Trial"),
                    cancellationToken);

            if (!hasValidSubscription)
            {
                throw new BadHttpRequestException(
                    "Cannot create a store. You must have an active or trial subscription.");
            }

            var normalizedDomain = request.ShopDomain.Trim().ToLower();
            var domainExists = await _dbContext.Stores
                .AnyAsync(s => s.ShopDomain.ToLower() == normalizedDomain && s.DeletedAt == null, cancellationToken);

            if (domainExists)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("ShopDomain", "A store with this shop domain already exists.")
                });
            }

            var store = new Store
            {
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Platform = request.Platform.Trim(),
                ShopDomain = request.ShopDomain.Trim(),
                Currency = request.Currency.Trim(),
                Language = request.Language.Trim(),
                Timezone = request.Timezone.Trim(),
                Status = "Active",
                UserId = userId
            };

            // Link store to user's Organization if it exists
            var organizationId = await _dbContext.Organizations
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.DeletedAt == null)
                .Select(o => (Guid?)o.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (organizationId.HasValue)
            {
                store.OrganizationId = organizationId.Value;
            }

            _dbContext.Stores.Add(store);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var defaultCapabilities = new StoreCapabilitiesDocument
            {
                StoreId = store.Id.ToString().ToLower(),
                Capabilities = new BsonDocument
                {
                    { "has_promo_code", false }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _mongoDbContext.StoreCapabilities.InsertOneAsync(defaultCapabilities, cancellationToken: cancellationToken);
            await _auditLogger.LogAsync("Store.Create", userId, store.Id.ToString(), cancellationToken);

            return StoreResponse.FromStore(store);
        }
    }
}
