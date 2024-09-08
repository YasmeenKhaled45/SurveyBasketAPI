using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Api.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string FirstName {  get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public List<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
