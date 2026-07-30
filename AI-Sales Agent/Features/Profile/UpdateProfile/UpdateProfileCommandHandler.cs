using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Features.Profile.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ApiResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;

        public UpdateProfileCommandHandler(ICurrentUserService currentUserService, UserManager<User> userManager)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<ApiResult> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is not { } userId)
            {
                return ApiResult.Failure("User is not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return ApiResult.Failure("User not found.");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ApiResult.Failure("Failed to update profile.", errors);
            }

            return ApiResult.Success("Profile updated successfully.");
        }
    }
}
