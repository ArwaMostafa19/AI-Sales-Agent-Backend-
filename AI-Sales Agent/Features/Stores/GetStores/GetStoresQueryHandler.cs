using AI_Sales_Agent.Data;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AI_Sales_Agent.Features.Stores.GetStores
{
    public class GetStoresQueryHandler : IRequestHandler<GetStoresQuery, IReadOnlyList<StoreResponse>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetStoresQueryHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<IReadOnlyList<StoreResponse>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is not { } userId)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            return await _dbContext.Stores
                .Where(store => store.UserId == userId && store.DeletedAt == null)
                .OrderBy(store => store.Name)
                .Select(store => StoreResponse.FromStore(store))
                .ToListAsync(cancellationToken);
        }
    }
}
