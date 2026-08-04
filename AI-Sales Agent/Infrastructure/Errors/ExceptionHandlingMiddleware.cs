using FluentValidation;

namespace AI_Sales_Agent.Infrastructure.Errors
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException exception)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Validation failed.",
                    errors = exception.Errors.Select(error => error.ErrorMessage)
                });
            }
            catch (UnauthorizedAccessException exception)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = exception.Message
                });
            }
            catch (BadHttpRequestException exception)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsJsonAsync(new
                {
                    message = exception.Message
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                object response = _environment.IsDevelopment()
                    ? new
                    {
                        message = "An unexpected error occurred.",
                        detail = exception.Message,
                        exceptionType = exception.GetType().Name
                    }
                    : new
                    {
                        message = "An unexpected error occurred."
                    };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
