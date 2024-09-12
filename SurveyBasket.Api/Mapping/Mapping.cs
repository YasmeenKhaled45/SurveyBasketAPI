using Mapster;
using Microsoft.AspNetCore.Identity.Data;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Mapping
{
    public class Mapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Poll, PollRequest>().TwoWays();
            config.NewConfig<QuestionRequest, Question>()
                .Map(dest => dest.Answers, src => src.Answers.Select(answer=> new Answer { Content = answer}));
            config.NewConfig<RegisterRequest, ApplicationUser>()
                .Map(dest => dest.UserName, src => src.Email);
        }

    }
}
