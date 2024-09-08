using Microsoft.EntityFrameworkCore;

namespace SurveyBasket.Api.Models
{
    [Owned]
    public class RefreshToken
    {
        public string Token {  get; set; }
        public DateTime ExpiresOn {  get; set; }
        public DateTime CreatedOn => DateTime.UtcNow;
        public DateTime? RevokedOn {  get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsActive => RevokedOn is null && !IsExpired;
    }
}
