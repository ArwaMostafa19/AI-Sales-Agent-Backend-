namespace AI_Sales_Agent.Abstractions
{
    public record AuthResult(
        string Token,
        string RefreshToken,
        DateTime IssuedAt,
        DateTime ExpiresAt,
        Guid UserId,
        Guid? StoreId,
        Guid? OrganizationId,
        string Email,
        string FirstName,
        string LastName,
        bool EmailConfirmed);
}
