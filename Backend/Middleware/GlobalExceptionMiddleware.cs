using System.Net;
using System.Text.Json;

namespace Backend.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request execution.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                message = "An unexpected server error occurred. Please try again later.",
                details = exception.Message
            };

            if (exception is InvalidOperationException || exception is ArgumentException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode = (int)HttpStatusCode.BadRequest,
                    message = exception.Message,
                    details = exception.Message
                };
            }
            else if (exception is KeyNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    statusCode = (int)HttpStatusCode.NotFound,
                    message = exception.Message,
                    details = exception.Message
                };
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
