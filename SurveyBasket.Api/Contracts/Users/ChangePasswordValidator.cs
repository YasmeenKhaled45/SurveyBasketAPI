using FluentValidation;

namespace SurveyBasket.Api.Contracts.Users
{
    public class ChangePasswordValidator : AbstractValidator<ChangePassword>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty()
                .NotEqual(x => x.CurrentPassword)
                .Matches("(?i)^(?=[a-z])(?=.*[0-9])([a-z0-9!@#$%\\^&*()_?+\\-=]){8,15}$")
               .WithMessage("Password must be at least 8 digits and should contain Lowercase, NonAlphanummeric and Uppercase")
               .NotEqual(x => x.CurrentPassword)
               .WithMessage("New Password can not be same as currentpassword!");

        }
    }
}
