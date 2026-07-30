using AI_Sales_Agent.Abstractions;
using MediatR;

namespace AI_Sales_Agent.Features.Profile.UpdateProfile
{
    public record UpdateProfileCommand(
        string FirstName,
        string LastName,
        string? PhoneNumber
    ) : IRequest<ApiResult>;
}
