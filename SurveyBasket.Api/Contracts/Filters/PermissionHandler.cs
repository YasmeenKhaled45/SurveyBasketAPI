using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Api.Constants;

namespace SurveyBasket.Api.Contracts.Filters
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
           

            if (context.User.Identity is not { IsAuthenticated: true } ||
                !context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == "permissions"))
                return;

            context.Succeed(requirement);
            return;
        }
    }
}
