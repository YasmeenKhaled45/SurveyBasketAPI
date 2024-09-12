using FluentValidation;
using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Validations
{
    public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
    {
        public QuestionRequestValidator() 
        {
            RuleFor(x => x.Content).NotEmpty()
                .Length(3, 1000);
            RuleFor(x => x.Answers).NotNull();

            RuleFor(x => x.Answers)
                .Must(x => x.Count > 1)
                .WithMessage("Questions must have at least two answers!")
                .When(x => x.Answers != null);

            RuleFor(x => x.Answers)
           .Must(x => x.Distinct().Count() == x.Count)
           .WithMessage("You can not add duplicated answers!")
             .When(x => x.Answers != null);
        }
    }
}
