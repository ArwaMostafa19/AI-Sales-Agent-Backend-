using MediatR;

namespace AI_Sales_Agent.Features.Auth.Account
{
    public record AccountResponse(
        Guid Id,
        string Email,
        string FirstName,
        string LastName,
        bool EmailConfirmed,
        DateTime? LastLogin);

    public record GetAccountQuery : IRequest<AccountResponse>;
}
