using AI_Sales_Agent.Abstractions;
using AI_Sales_Agent.Domain;
using AI_Sales_Agent.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AI_Sales_Agent.Features.Profile.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResult>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;

        public ChangePasswordCommandHandler(ICurrentUserService currentUserService, UserManager<User> userManager)
        {
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<ApiResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
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

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return ApiResult.Failure("Failed to change password.", errors);
            }

            return ApiResult.Success("Password changed successfully.");
        }
    }
}
