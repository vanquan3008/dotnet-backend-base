using MyProject.Application.Common.Results;

namespace MyProject.Application.Common.Exceptions;

public sealed class ForbiddenException(string code, string message)
    : ApplicationExceptionBase(code, message, ErrorType.Forbidden);
