using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Contracts.JWT
{
    public interface IJWTProvider
    {
        (string Token, int ExpiresIn) GenerateToken(ApplicationUser user,IEnumerable<string> Roles , IEnumerable<string> permissions);
        string? ValidateToken(string token);
    }
}
