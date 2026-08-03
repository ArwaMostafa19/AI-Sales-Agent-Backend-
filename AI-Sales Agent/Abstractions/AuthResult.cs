namespace AI_Sales_Agent.Abstractions
{
    public record AuthResult(
        string Token,
        string RefreshToken,
        DateTime IssuedAt,
        DateTime ExpiresAt,
        Guid UserId,
        Guid? StoreId,
        string Email,
        string FirstName,
        string LastName,
        bool EmailConfirmed);
}
