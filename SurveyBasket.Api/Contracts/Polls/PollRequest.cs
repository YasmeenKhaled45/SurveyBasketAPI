namespace SurveyBasket.Api.Contracts.Polls
{
    public record PollRequest(
        string Title,
        string Description,
        DateOnly StartsAt,
        DateOnly EndsAt
        );

}
