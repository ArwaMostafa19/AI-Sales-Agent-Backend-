using AI_Sales_Agent.Abstractions;
using MediatR;

namespace AI_Sales_Agent.Features.Auth.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;
}
