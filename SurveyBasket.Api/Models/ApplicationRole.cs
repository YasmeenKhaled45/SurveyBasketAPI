using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Api.Models
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsDefault {  get; set; }
        public bool IsDeleted {  get; set; }
    }
}
