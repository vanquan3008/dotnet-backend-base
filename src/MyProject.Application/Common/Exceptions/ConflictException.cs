using MyProject.Application.Common.Results;

namespace MyProject.Application.Common.Exceptions;

public sealed class ConflictException(string code, string message, Exception? innerException = null)
    : ApplicationExceptionBase(code, message, ErrorType.Conflict, innerException);
