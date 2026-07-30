using AI_Sales_Agent.Abstractions;
using MediatR;

namespace AI_Sales_Agent.Features.Auth.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<ApiResult>;
}
