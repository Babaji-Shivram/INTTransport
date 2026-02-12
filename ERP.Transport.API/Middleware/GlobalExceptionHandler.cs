using ERP.Transport.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ERP.Transport.API.Middleware;

/// <summary>
/// Global exception handler middleware — catches all unhandled exceptions and
/// returns RFC 7807 ProblemDetails JSON. Matches Workflow MS pattern.
/// 
/// Maps:
///   TransportNotFoundException    → 404
///   TransportBusinessException    → 422
///   TransportValidationException  → 400
///   TransportAccessDeniedException→ 403
///   ValidationException (Fluent)  → 400
///   KeyNotFoundException          → 404 (legacy compat)
///   InvalidOperationException     → 422 (legacy compat)
///   UnauthorizedAccessException   → 401
///   OperationCanceledException    → 499
///   Everything else               → 500
/// </summary>
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(
        RequestDelegate next,
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = exception switch
        {
            TransportNotFoundException notFound => (
                StatusCodes.Status404NotFound,
                CreateProblemDetails(StatusCodes.Status404NotFound,
                    "Resource Not Found", notFound.Message, notFound.ErrorCode)),

            TransportValidationException validation => (
                StatusCodes.Status400BadRequest,
                CreateValidationProblemDetails(validation)),

            TransportBusinessException business => (
                StatusCodes.Status422UnprocessableEntity,
                CreateProblemDetails(StatusCodes.Status422UnprocessableEntity,
                    "Business Rule Violation", business.Message, business.ErrorCode)),

            TransportAccessDeniedException access => (
                StatusCodes.Status403Forbidden,
                CreateProblemDetails(StatusCodes.Status403Forbidden,
                    "Forbidden", access.Message, access.ErrorCode)),

            ValidationException fluentValidation => (
                StatusCodes.Status400BadRequest,
                CreateFluentValidationProblemDetails(fluentValidation)),

            // Legacy exception compatibility
            KeyNotFoundException keyNotFound => (
                StatusCodes.Status404NotFound,
                CreateProblemDetails(StatusCodes.Status404NotFound,
                    "Resource Not Found", keyNotFound.Message, "ENTITY_NOT_FOUND")),

            InvalidOperationException invalidOp => (
                StatusCodes.Status422UnprocessableEntity,
                CreateProblemDetails(StatusCodes.Status422UnprocessableEntity,
                    "Invalid Operation", invalidOp.Message, "INVALID_OPERATION")),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                CreateProblemDetails(StatusCodes.Status401Unauthorized,
                    "Unauthorized", "Authentication required", "UNAUTHORIZED")),

            OperationCanceledException => (
                499, // Client Closed Request
                CreateProblemDetails(499,
                    "Request Cancelled", "The request was cancelled by the client", "REQUEST_CANCELLED")),

            _ => (
                StatusCodes.Status500InternalServerError,
                CreateProblemDetails(StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    _env.IsDevelopment() ? exception.ToString() : "An unexpected error occurred",
                    "INTERNAL_ERROR"))
        };

        // Log at appropriate level
        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else if (statusCode >= 400)
            _logger.LogWarning("Client error {StatusCode}: {Message}", statusCode, exception.Message);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }

    private static ProblemDetails CreateProblemDetails(
        int statusCode, string title, string detail, string errorCode)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}",
            Extensions = { ["errorCode"] = errorCode, ["timestamp"] = DateTime.UtcNow }
        };
    }

    private static ProblemDetails CreateValidationProblemDetails(TransportValidationException ex)
    {
        return new ValidationProblemDetails(ex.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Failed",
            Detail = ex.Message,
            Type = "https://httpstatuses.com/400",
            Extensions = { ["errorCode"] = ex.ErrorCode, ["timestamp"] = DateTime.UtcNow }
        };
    }

    private static ProblemDetails CreateFluentValidationProblemDetails(ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Failed",
            Detail = "One or more validation errors occurred",
            Type = "https://httpstatuses.com/400",
            Extensions = { ["errorCode"] = "VALIDATION_FAILED", ["timestamp"] = DateTime.UtcNow }
        };
    }
}
