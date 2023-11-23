using System.Text.Json;
using RacingFeedApi.Exceptions;


namespace RacingFeedApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            switch (exception)
            {
                case ResourceNotFoundException _:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.ContentType = "application/json";
                    //TODO: LogInformation(context, exception.Message);
                    return context.Response.WriteAsync(FormatErrorResponse(StatusCodes.Status404NotFound, exception.Message));
                case CreateResourceException _:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    //TODO: LogInformation(context, exception.Message);
                    return context.Response.WriteAsync(FormatErrorResponse(StatusCodes.Status500InternalServerError, exception.Message));
                case UpdateResourceException _:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    // TODO: LogInformation(context, exception.Message);
                    return context.Response.WriteAsync(FormatErrorResponse(StatusCodes.Status500InternalServerError, exception.Message));
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    // TODO: LogException(context, exception);
                    return context.Response.WriteAsync(FormatErrorResponse(StatusCodes.Status500InternalServerError, "Server error"));
            }
        }

        static string FormatErrorResponse(int status, string message)
        {
            return JsonSerializer.Serialize(new
            {
                // customize as you need
                error = new
                {
                    message,
                    status
                }
            });
        }
    }
}
