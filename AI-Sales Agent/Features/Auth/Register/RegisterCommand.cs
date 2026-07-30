using AI_Sales_Agent.Abstractions;
using MediatR;

namespace AI_Sales_Agent.Features.Auth.Register
{
    public record RegisterCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string ConfirmPassword) : IRequest<ApiResult>;
}
