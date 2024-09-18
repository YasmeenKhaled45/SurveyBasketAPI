namespace SurveyBasket.Api.Contracts.Roles
{
    public record RoleDetailResponse
    (
    string Id,
    string Name,
    IEnumerable<string> Permissions
    );
}
