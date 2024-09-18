using FluentValidation;

namespace SurveyBasket.Api.Contracts.Authentication
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password).NotEmpty()
                .Matches("(?i)^(?=[a-z])(?=.*[0-9])([a-z0-9!@#$%\\^&*()_?+\\-=]){8,15}$")
                .WithMessage("Password must be at least 8 digits and should contain Lowercase, NonAlphanummeric and Uppercase");
            RuleFor(x => x.FirstName).NotEmpty()
                .Length(3, 100);
            RuleFor(x => x.LastName).NotEmpty().
              Length(3, 100);
        }
    }
}
