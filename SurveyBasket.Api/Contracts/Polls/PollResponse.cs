namespace SurveyBasket.Api.Contracts.Polls
{
    public record PollResponse(
        int Id,
        string Title,
        string Description,
        DateOnly StartsAt,
        DateOnly EndsAt,
        bool Ispublished
        );

}
