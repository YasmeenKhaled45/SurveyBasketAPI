using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public interface IPollService
    {
        Task<IEnumerable<Poll>> GetAll(CancellationToken cancellationToken=default);
        Task<Poll> Add(Poll poll, CancellationToken cancellation = default);
        Task<bool> Update(int Id, Poll poll ,CancellationToken cancellationToken = default);
        Task<bool> Delete(int Id,CancellationToken cancellationToken = default);
        Task<bool> TogglePublishStatus(int Id , CancellationToken cancellationToken = default);
    }
}
