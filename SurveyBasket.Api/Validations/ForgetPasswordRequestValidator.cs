using FluentValidation;
using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Validations
{
    public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordRequestValidator() 
        {
            RuleFor(x => x.Email).NotEmpty()
               .EmailAddress();
        }
    }
}
