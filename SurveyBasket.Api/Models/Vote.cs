namespace SurveyBasket.Api.Models
{
    public sealed class Vote 
    {
        public int Id { get; set; }
        public int PollId {  get; set; }
        public string UserId {  get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;
        public Poll Poll { get; set; } = default!;
        public DateTime SubmittedOn => DateTime.UtcNow;
        public ICollection<VoteAnswer> Answers { get; set; } = [];   
    }
}
