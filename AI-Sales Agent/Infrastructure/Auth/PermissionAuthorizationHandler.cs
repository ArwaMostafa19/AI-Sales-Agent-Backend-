using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AI_Sales_Agent.Infrastructure.Auth;

namespace AI_Sales_Agent.Infrastructure.Auth
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Roles.Admin))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var hasPermission = context.User.Claims.Any(claim =>
                claim.Type == "permission" && claim.Value == requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
