using AI_Sales_Agent.Data;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Stores.GetStore
{
    public class GetStoreQueryHandler : IRequestHandler<GetStoreQuery, StoreResponse?>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetStoreQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<StoreResponse?> Handle(GetStoreQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is not { } userId)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var store = await _dbContext.Stores
                .FirstOrDefaultAsync(store =>
                    store.Id == request.StoreId
                    && store.UserId == userId
                    && store.DeletedAt == null,
                    cancellationToken);

            return store is null ? null : StoreResponse.FromStore(store);
        }
    }
}
