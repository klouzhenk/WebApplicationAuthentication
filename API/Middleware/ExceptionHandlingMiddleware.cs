using Microsoft.AspNetCore.Mvc;
using API.Infrastructure;
using Serilog;
using System.Diagnostics;
using System;

namespace API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            Log.Information("\n\nStart process ---------------------------\n");
            await _next(context);
            stopwatch.Stop();
            Log.Information($"\n\nFinish process ---------------------------\nElapsed Time: {stopwatch.ElapsedMilliseconds} ms\n");
        }
        catch (CustomException exception)
        {
            stopwatch.Stop();
            Log.Error(exception, $"Custom exception caught in middleware. Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
            var problemDetails = CreateProblemDetails(StatusCodes.Status400BadRequest, "Client Error", exception.Message);

			context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            Log.Error(exception, $"An unhandled exception has occurred. Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
			var problemDetails = CreateProblemDetails(StatusCodes.Status500InternalServerError, "Server Error", "An unexpected error occurred");

			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    private static ProblemDetails CreateProblemDetails(int status, string title, string detail)
    {
		return new ProblemDetails
		{
			Status = status,
			Title = title,
			Detail = detail
		};
	}
}
