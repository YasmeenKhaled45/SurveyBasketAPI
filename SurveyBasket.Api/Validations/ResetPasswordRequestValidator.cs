using FluentValidation;
using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Validations
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator() 
        {
            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Code).NotEmpty();

            RuleFor(x => x.NewPassword).NotEmpty()
           .Matches("(?i)^(?=[a-z])(?=.*[0-9])([a-z0-9!@#$%\\^&*()_?+\\-=]){8,15}$")
          .WithMessage("Password must be at least 8 digits and should contain Lowercase, NonAlphanummeric and Uppercase");

        }
    }
}
