using FluentValidation;

namespace SurveyBasket.Api.Contracts.Authentication
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
