namespace ERP.Transport.Domain.Exceptions;

/// <summary>
/// Base exception for transport domain-specific errors.
/// Carries an error code for structured ProblemDetails responses.
/// </summary>
public abstract class TransportDomainException : Exception
{
    public string ErrorCode { get; }

    protected TransportDomainException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    protected TransportDomainException(string errorCode, string message, Exception inner)
        : base(message, inner)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Entity not found — maps to HTTP 404.
/// </summary>
public class TransportNotFoundException : TransportDomainException
{
    public string EntityName { get; }
    public object EntityId { get; }

    public TransportNotFoundException(string entityName, object entityId)
        : base("ENTITY_NOT_FOUND", $"{entityName} with ID '{entityId}' was not found")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}

/// <summary>
/// Business rule violation — maps to HTTP 422.
/// </summary>
public class TransportBusinessException : TransportDomainException
{
    public TransportBusinessException(string message)
        : base("BUSINESS_RULE_VIOLATION", message) { }

    public TransportBusinessException(string errorCode, string message)
        : base(errorCode, message) { }
}

/// <summary>
/// Validation failure — maps to HTTP 400.
/// </summary>
public class TransportValidationException : TransportDomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public TransportValidationException(IDictionary<string, string[]> errors)
        : base("VALIDATION_FAILED", "One or more validation errors occurred")
    {
        Errors = errors;
    }

    public TransportValidationException(string field, string error)
        : base("VALIDATION_FAILED", error)
    {
        Errors = new Dictionary<string, string[]> { { field, new[] { error } } };
    }
}

/// <summary>
/// Access denied — maps to HTTP 403.
/// </summary>
public class TransportAccessDeniedException : TransportDomainException
{
    public TransportAccessDeniedException(string message = "Access denied")
        : base("ACCESS_DENIED", message) { }
}
