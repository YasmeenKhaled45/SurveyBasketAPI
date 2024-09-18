namespace SurveyBasket.Api.Abstractions
{
    public record RequestFilters
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SortColumn { get; init; }
        public string? SortDirection { get; init; } = "ASC";

    }
}
