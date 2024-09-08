using Mapster;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Mapping
{
    public class Mapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Poll, PollRequest>().TwoWays();
        }

    }
}
