using AI_Sales_Agent.Data;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Stores.UpdateStoreSettings
{
    public class UpdateStoreSettingsCommandHandler : IRequestHandler<UpdateStoreSettingsCommand, StoreResponse?>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditLogger _auditLogger;

        public UpdateStoreSettingsCommandHandler(
            ApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IAuditLogger auditLogger)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _auditLogger = auditLogger;
        }

        public async Task<StoreResponse?> Handle(UpdateStoreSettingsCommand request, CancellationToken cancellationToken)
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

            store.Currency = request.Currency.Trim();
            store.Language = request.Language.Trim();
            store.Timezone = request.Timezone.Trim();
            store.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _auditLogger.LogAsync("Store.Settings.Update", userId, store.Id.ToString(), cancellationToken);

            return StoreResponse.FromStore(store);
        }
    }
}
