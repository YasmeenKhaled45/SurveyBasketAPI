namespace SurveyBasket.Api.Models
{
    public sealed class Answer
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int QuestionId { get; set; }
        public Question Question { get; set; } = default!;
        public bool IsActive { get; set; } = true;

    }
}
