using FluentValidation;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Validations
{
    public class VoteRequestValidator : AbstractValidator<VoteRequest>
    {
        public VoteRequestValidator() 
        {
            RuleFor(x=>x.Answers).NotEmpty();
        }
    }
}
