namespace SurveyBasket.Api.Abstractions
{
    public record Error(string Code ,  string Message)
    {
        public static readonly Error none = new Error(string.Empty, string.Empty);
    }
}
