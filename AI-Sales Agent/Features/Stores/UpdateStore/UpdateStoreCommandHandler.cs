using AI_Sales_Agent.Data;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.Results;

namespace AI_Sales_Agent.Features.Stores.UpdateStore
{
    public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, StoreResponse?>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditLogger _auditLogger;

        public UpdateStoreCommandHandler(
            ApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IAuditLogger auditLogger)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _auditLogger = auditLogger;
        }

        public async Task<StoreResponse?> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is not { } userId)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var store = await _dbContext.Stores.FirstOrDefaultAsync(store =>
                store.Id == request.StoreId
                && store.UserId == userId
                && store.DeletedAt == null,
                cancellationToken);

            if (store is null)
            {
                return null;
            }

            var normalizedDomain = request.ShopDomain.Trim().ToLower();
            var domainExists = await _dbContext.Stores
                .AnyAsync(s => s.Id != request.StoreId && s.ShopDomain.ToLower() == normalizedDomain && s.DeletedAt == null, cancellationToken);

            if (domainExists)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("ShopDomain", "A store with this shop domain already exists.")
                });
            }

            store.Name = request.Name.Trim();
            store.Description = request.Description.Trim();
            store.Platform = request.Platform.Trim();
            store.ShopDomain = request.ShopDomain.Trim();
            store.Status = request.Status.Trim();
            store.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _auditLogger.LogAsync("Store.Update", userId, store.Id.ToString(), cancellationToken);

            return StoreResponse.FromStore(store);
        }
    }
}
