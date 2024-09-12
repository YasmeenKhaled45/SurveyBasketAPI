using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Abstractions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger): IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
                _logger.LogError(exception, "Something went wrong: {Message}", exception.Message);
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error!",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
                };
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken:cancellationToken);
            return true;
        }
    }
}
