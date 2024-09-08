using FluentValidation;
using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Validations
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator() 
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x=>x.RefreshToken).NotEmpty();  
        }
    }
}
