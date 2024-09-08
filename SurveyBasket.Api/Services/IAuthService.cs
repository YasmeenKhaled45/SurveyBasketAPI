using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> GetTokenAsync(string Email , string Password , CancellationToken cancellationToken = default);
        Task<AuthResponse?> GetRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default);
        Task<bool> RevokeTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default);
    }
}
