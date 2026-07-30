using AI_Sales_Agent.Data;
using AI_Sales_Agent.Infrastructure.Audit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Admin.Stores.UpdateStoreStatus;

public class UpdateStoreStatusHandler : IRequestHandler<UpdateStoreStatusCommand, bool>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IAuditLogger _auditLogger;

    public UpdateStoreStatusHandler(ApplicationDbContext dbContext, IAuditLogger auditLogger)
    {
        _dbContext = dbContext;
        _auditLogger = auditLogger;
    }

    public async Task<bool> Handle(UpdateStoreStatusCommand request, CancellationToken cancellationToken)
    {
        var store = await _dbContext.Stores
            .FirstOrDefaultAsync(store => store.Id == request.StoreId && store.DeletedAt == null, cancellationToken);

        if (store is null)
        {
            return false;
        }

        store.Status = request.Status.Trim();
        store.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLogger.LogAsync(
            "AdminStoreStatusUpdated",
            metadata: $"StoreId={store.Id};Status={store.Status};Reason={request.Reason}",
            cancellationToken: cancellationToken);

        return true;
    }
}
