using MyProject.Application.Common.Exceptions;
using MyProject.Domain.Exceptions;

namespace MyProject.Application.Common.Results;

public static class ExceptionErrorExtensions
{
    public static Error ToError(this DomainException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return exception.Type switch
        {
            DomainErrorType.Validation => Error.Validation(exception.Code, exception.Message),
            DomainErrorType.Conflict => Error.Conflict(exception.Code, exception.Message),
            _ => Error.Failure(exception.Code, exception.Message)
        };
    }

    public static Error ToError(this ApplicationExceptionBase exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return exception.Type switch
        {
            ErrorType.Validation => Error.Validation(exception.Code, exception.Message),
            ErrorType.NotFound => Error.NotFound(exception.Code, exception.Message),
            ErrorType.Conflict => Error.Conflict(exception.Code, exception.Message),
            ErrorType.Forbidden => Error.Forbidden(exception.Code, exception.Message),
            ErrorType.Unauthorized => Error.Unauthorized(exception.Code, exception.Message),
            _ => Error.Failure(exception.Code, exception.Message)
        };
    }
}
