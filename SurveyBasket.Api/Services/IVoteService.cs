using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Services
{
    public interface IVoteService
    {
        Task<Result> SaveVote(int PollID,string UserId, VoteRequest request , CancellationToken cancellationToken = default);

    }
}
