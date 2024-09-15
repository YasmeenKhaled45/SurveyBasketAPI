using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Api.Contracts.Filters
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
