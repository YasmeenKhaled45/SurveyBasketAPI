namespace SurveyBasket.Api.Contracts.Results
{
    public record PollVotesResponses
   (string Title , IEnumerable<VoteResponse> VoteResponses);
}
