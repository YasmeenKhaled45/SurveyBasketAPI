using FluentValidation;

namespace SurveyBasket.Api.Contracts.Polls
{
    public class PollsValidation : AbstractValidator<PollRequest>
    {
        public PollsValidation()
        {
            RuleFor(x => x.Title).
                NotEmpty()
                .WithMessage("Please add a {PropertyName}!")
                .Length(3, 5);

            RuleFor(x => x.Description).NotEmpty()
                .Length(3, 1000);
            RuleFor(x => x.StartsAt).NotEmpty()
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
            RuleFor(x => x.EndsAt).NotEmpty();
            RuleFor(x => x).Must(HasValidDates)
                .WithName(nameof(PollRequest.EndsAt))
                .WithMessage("{PropertyName} must be greater than or equal StartsAt date");
        }
        private bool HasValidDates(PollRequest poll)
        {
            return poll.EndsAt >= poll.StartsAt;
        }
    }
}
