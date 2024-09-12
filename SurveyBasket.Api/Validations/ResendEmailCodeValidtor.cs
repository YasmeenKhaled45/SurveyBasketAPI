using FluentValidation;
using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Validations
{
    public class ResendEmailCodeValidtor : AbstractValidator<ResendEmailCode>
    {
        public ResendEmailCodeValidtor() 
        {
            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress();
        }
    }
}
