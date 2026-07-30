namespace AI_Sales_Agent.Features.Profile.GetProfile
{
    public record ProfileResponseDto(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        string? PhoneNumber,
        string? ProfilePictureUrl,
        DateTime? LastLogin,
        List<ProfileStoreDto> Stores,
        ProfileSubscriptionDto? Subscription
    );

    public record ProfileStoreDto(
        Guid StoreId,
        string Name,
        string Description,
        string Platform,
        string ShopDomain,
        string Currency,
        string Language,
        string Timezone,
        string Status
    );

    public record ProfileSubscriptionDto(
        Guid SubscriptionId,
        string Status,
        DateTime? RenewalDate,
        Guid PlanId,
        string PlanName,
        decimal PlanPrice
    );
}
