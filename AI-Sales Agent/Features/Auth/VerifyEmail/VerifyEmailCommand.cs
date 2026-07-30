using AI_Sales_Agent.Abstractions;
using MediatR;

namespace AI_Sales_Agent.Features.Auth.VerifyEmail
{
    public record VerifyEmailCommand(Guid UserId, string Token) : IRequest<ApiResult>;
}
