using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public interface IPollService
    {
        Task<IEnumerable<PollResponse>> GetAll(CancellationToken cancellationToken=default);
        Task<IEnumerable<PollResponse>> GetAvailablePolls(CancellationToken cancellationToken = default);
        Task <Result<PollResponse>> Add(PollRequest poll, CancellationToken cancellation = default);
        Task<Result> Update(int Id, PollRequest poll ,CancellationToken cancellationToken = default);
        Task<Result> Delete(int Id,CancellationToken cancellationToken = default);
        Task<Result> TogglePublishStatus(int Id , CancellationToken cancellationToken = default);
    }
}
