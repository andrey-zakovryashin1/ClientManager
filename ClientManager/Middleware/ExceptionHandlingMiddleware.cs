using System.Net;
using System.Text.Json;

namespace ClientManager.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally in the application.
    /// Catches unhandled exceptions and returns a standardized JSON response.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invokes the middleware to process the HTTP request.
        /// </summary>
        /// <param name="context">The HTTP context for the request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the exception and returns a JSON response with error details.
        /// </summary>
        /// <param name="context">The HTTP context for the request.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                error = "An error occurred while processing your request.",
                details = exception.Message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}