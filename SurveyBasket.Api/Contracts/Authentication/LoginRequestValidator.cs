using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;

namespace SurveyBasket.Api.Contracts.Authentication
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
