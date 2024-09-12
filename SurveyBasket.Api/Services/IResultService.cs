using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Results;

namespace SurveyBasket.Api.Services
{
    public interface IResultService
    {
       Task<Result<PollVotesResponses>> GetPollVotes(int PollId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<VotesPerDayResponse>>> VotesPerDay(int PollId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<VotesPerQuestion>>> VotesPerQuestion(int PollId, CancellationToken cancellationToken = default);
    }
}
