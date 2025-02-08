using Domain.Exceptions;
using Domain.Exceptions.Products;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An unhandled exception occurred");

        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = "An error occurred while processing your request",
            Detail = ex.Message,
            Status = StatusCodes.Status500InternalServerError
        };

        (Int32 status, String detail) = ex switch
        {
            CustomerDetailsNotFoundException => (StatusCodes.Status400BadRequest, "Address or card details have not been set. Please enter them and try again."),
            InsufficientStockException ise => (StatusCodes.Status400BadRequest, $"Insufficient stock for product '{ise.ProductName}'. Requested: {ise.Requested} | Available: {ise.Available}."),
            _ => (StatusCodes.Status500InternalServerError, "An error occured when trying to process your request. Please try again later.")
        };

        problemDetails.Status = status;
        problemDetails.Title = ex.GetType().Name;
        problemDetails.Detail = detail;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}