namespace SurveyBasket.Api.Models
{
    public class Poll : AuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Ispublished {  get; set; }
        public DateOnly StartsAt { get; set; }
        public DateOnly EndsAt { get; set; }
        public ICollection<Question> Questions { get; set; } = [];
        public ICollection<Vote> Votes { get; set; } = [];

    }
}
