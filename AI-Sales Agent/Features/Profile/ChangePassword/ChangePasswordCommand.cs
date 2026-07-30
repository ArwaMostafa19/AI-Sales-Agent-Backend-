using AI_Sales_Agent.Abstractions;
using MediatR;

namespace AI_Sales_Agent.Features.Profile.ChangePassword
{
    public record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    ) : IRequest<ApiResult>;
}
