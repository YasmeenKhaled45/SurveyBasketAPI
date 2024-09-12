using FluentValidation;
using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Validations
{
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(x=>x.UserId).NotEmpty();
            RuleFor(x=>x.Code).NotEmpty();
        }
    }
}
