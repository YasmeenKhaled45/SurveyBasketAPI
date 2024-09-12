using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string Email , string Password , CancellationToken cancellationToken = default);
        Task<AuthResponse?> GetRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default);
        Task<Result> RegisterAsync(RegisterRequest register , CancellationToken cancellationToken = default); 
        Task<bool> RevokeTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<Result> ResendEmailCodeAsync(ResendEmailCode resendEmail);
    }
}
