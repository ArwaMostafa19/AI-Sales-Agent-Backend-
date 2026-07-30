using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Data;
using AI_Sales_Agent.Infrastructure.Audit;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Stores.DeleteStore
{
    public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, ApiResult>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditLogger _auditLogger;

        public DeleteStoreCommandHandler(
            ApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            IAuditLogger auditLogger)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResult> Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
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
                return ApiResult.Failure("Store not found.");
            }

            store.DeletedAt = DateTime.UtcNow;
            store.Status = "Deleted";

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _auditLogger.LogAsync("Store.Delete", userId, store.Id.ToString(), cancellationToken);

            return ApiResult.Success("Store deleted successfully.");
        }
    }
}
