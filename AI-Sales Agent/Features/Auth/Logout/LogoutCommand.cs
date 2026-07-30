using AI_Sales_Agent.Abstractions;
using MediatR;

namespace AI_Sales_Agent.Features.Auth.Logout
{
    public record LogoutCommand : IRequest<ApiResult>;
}
