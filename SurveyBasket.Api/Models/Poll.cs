namespace SurveyBasket.Api.Models
{
    public class Poll : AuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Ispublished {  get; set; }
        public DateOnly StartsAt { get; set; }
        public DateOnly EndsAt { get; set; }
    }
}
