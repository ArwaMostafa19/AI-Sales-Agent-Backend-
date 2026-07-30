namespace AI_Sales_Agent.Abstractions
{
    public record AuthResult(
        string Token,
        string RefreshToken,
        DateTime ExpiresAt,
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        bool EmailConfirmed);
}
