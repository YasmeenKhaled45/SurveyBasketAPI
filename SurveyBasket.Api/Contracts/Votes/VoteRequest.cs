using Microsoft.Identity.Client;

namespace SurveyBasket.Api.Contracts.Votes
{
    public record VoteRequest
    (
       IEnumerable<VoteAnswerRequest> Answers
    );
}
