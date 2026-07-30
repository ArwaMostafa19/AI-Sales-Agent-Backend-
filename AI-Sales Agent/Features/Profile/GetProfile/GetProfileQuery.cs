using MediatR;

namespace AI_Sales_Agent.Features.Profile.GetProfile
{
    public record GetProfileQuery() : IRequest<ProfileResponseDto>;
}
