using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.Api.Contracts.JWT
{
    public class JWTOptions
    {
        [Required]
        public string key { get; init; }
        [Required]
        public string Issuer { get; init; }
        [Required]
        public string Audience { get; init; }
        [Required]
        [Range(1, int.MaxValue)]
        public int ExpiryMinutes { get; init; }
    }
}
