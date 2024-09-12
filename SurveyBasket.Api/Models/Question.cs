namespace SurveyBasket.Api.Models
{
    public sealed class Question : AuditableEntity
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int PollId { get; set; }
        public Poll Poll { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public ICollection<Answer> Answers { get; set; } = [];
        public ICollection<VoteAnswer> Votes { get; set; } = [];
    }
}
