namespace SurveyBasket.Api.Contracts.Authentication
{
    public record RefreshTokenRequest(
        string Token,
        string RefreshToken
     );
}
