using FluentValidation;

namespace SurveyBasket.Api.Contracts.Users
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfile>
    {
        public UpdateProfileValidator()
        {

            RuleFor(x => x.FirstName).NotEmpty()
                .Length(3, 100);
            RuleFor(x => x.LastName).NotEmpty().
              Length(3, 100);
        }

    }
}
