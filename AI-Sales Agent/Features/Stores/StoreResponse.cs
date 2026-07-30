using AI_Sales_Agent.Domain;

namespace AI_Sales_Agent.Features.Stores
{
    public record StoreResponse(
        Guid Id,
        string Name,
        string Description,
        string Platform,
        string ShopDomain,
        string Currency,
        string Language,
        string Timezone,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt)
    {
        public static StoreResponse FromStore(Store store)
        {
            return new StoreResponse(
                store.Id,
                store.Name,
                store.Description,
                store.Platform,
                store.ShopDomain,
                store.Currency,
                store.Language,
                store.Timezone,
                store.Status,
                store.CreatedAt,
                store.UpdatedAt);
        }
    }
}
