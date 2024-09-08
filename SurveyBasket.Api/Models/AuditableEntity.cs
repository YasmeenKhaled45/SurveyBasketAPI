namespace SurveyBasket.Api.Models
{
    public class AuditableEntity
    {
        public string CreateById {  get; set; }
        public ApplicationUser CreatedBy { get; set; }
         public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
         public DateTime? UpdatedOn {  get; set; } 
         public string? UpdatedById {  get; set; }
        public ApplicationUser? UpdatedBy { get; set; }

    }
}
