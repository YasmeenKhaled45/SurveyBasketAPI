using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Contracts.Authentication;
using SurveyBasket.Api.Contracts.JWT;
using SurveyBasket.Api.Models;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager,IJWTProvider jWTProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJWTProvider jWTProvider1 = jWTProvider;
        private readonly int refreshtokenexpiration = 14;
        public async Task<AuthResponse?> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return null;
            }
            var validpassword = await _userManager.CheckPasswordAsync(user, Password);
            if (!validpassword)
                return null;

            var (Token, ExpiresIn) = jWTProvider1.GenerateToken(user);
            var refreshtoken = GenereateRefreshToken();
            var refreshtokenExpiration = DateTime.UtcNow.AddDays(refreshtokenexpiration);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshtoken,
                ExpiresOn = refreshtokenExpiration, 
            });
            await _userManager.UpdateAsync(user);
            return new AuthResponse(user.Id,user.FirstName,user.LastName,user.Email,Token,ExpiresIn,refreshtoken,refreshtokenExpiration);
        }
        public async Task<AuthResponse?> GetRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default)
        {
             var userId = jWTProvider1.ValidateToken(Token);
            if (userId == null)
                return null;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return null;

            var userfrereshtoken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);
            if(userfrereshtoken is null)
                return null;

            userfrereshtoken.RevokedOn = DateTime.UtcNow;
            var (newtoken, ExpiresIn) = jWTProvider1.GenerateToken(user);
            var newrefreshtoken = GenereateRefreshToken();
            var refreshtokenExpiration = DateTime.UtcNow.AddDays(refreshtokenexpiration);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newrefreshtoken,
                ExpiresOn = refreshtokenExpiration,
            });
            await _userManager.UpdateAsync(user);
            return new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, newtoken, ExpiresIn, newrefreshtoken, refreshtokenExpiration);
        }
        private static string GenereateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<bool> RevokeTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default)
        {
            var userId = jWTProvider1.ValidateToken(Token);
            if (userId == null)
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return false;

            var userfrereshtoken = user.RefreshTokens.SingleOrDefault(x => x.Token == RefreshToken && x.IsActive);
            if (userfrereshtoken is null)
                return false;

            userfrereshtoken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }
    }

}
