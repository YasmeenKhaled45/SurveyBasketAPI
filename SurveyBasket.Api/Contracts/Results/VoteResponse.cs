namespace SurveyBasket.Api.Contracts.Results
{
    public record VoteResponse
    (
        string VoterName,
        DateTime VotingDate,
        IEnumerable<QuestionAnswerResponse> selectedAnswers
    );
}
