using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Api.Contracts.Filters
{
    public class HasPermission(string permission) : AuthorizeAttribute(permission)
    {

    }
}
