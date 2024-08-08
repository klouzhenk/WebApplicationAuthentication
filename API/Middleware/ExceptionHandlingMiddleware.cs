using Microsoft.AspNetCore.Mvc;
using API.Infrastructure;
using Serilog;
using System.Diagnostics;

namespace API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();  // Запускаємо таймер

            try
            {
                Log.Information("\n\nStart process ---------------------------\n");

                await _next(context);

                stopwatch.Stop();  // Зупиняємо таймер після завершення процесу
                Log.Information($"\n\nFinish process ---------------------------\nElapsed Time: {stopwatch.ElapsedMilliseconds} ms\n");
            }
            catch (CustomException exception)
            {
                stopwatch.Stop();  // Зупиняємо таймер у випадку виключення

                Log.Error(exception, $"Custom exception caught in middleware. Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Client Error",
                    Detail = exception.Message
                };

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (Exception exception)
            {
                stopwatch.Stop();  // Зупиняємо таймер у випадку виключення

                Log.Error(exception, $"An unhandled exception has occurred. Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server Error",
                    Detail = "An unexpected error occurred"
                };

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
