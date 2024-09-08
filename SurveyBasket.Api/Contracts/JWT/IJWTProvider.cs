using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Contracts.JWT
{
    public interface IJWTProvider
    {
        (string Token, int ExpiresIn) GenerateToken(ApplicationUser user);
        string? ValidateToken(string token);
    }
}
