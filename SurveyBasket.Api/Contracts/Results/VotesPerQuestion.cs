namespace SurveyBasket.Api.Contracts.Results
{
    public record VotesPerQuestion
    (string Question, IEnumerable<VotesPerAnswer> VotesPerAnswers);
    
}
