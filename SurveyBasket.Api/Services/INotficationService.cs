namespace SurveyBasket.Api.Services
{
    public interface INotficationService
    {
        Task SendPollNotfication(int? pollId = null);
    }
}
