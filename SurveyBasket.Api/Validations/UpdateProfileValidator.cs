using FluentValidation;
using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Validations
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
