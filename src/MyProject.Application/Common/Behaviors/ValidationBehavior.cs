using System.ComponentModel.DataAnnotations;
using MediatR;
using MyProject.Application.Common.Exceptions;

namespace MyProject.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var results = new List<ValidationResult>();
        if (Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true)
        )
        {
            return next(cancellationToken);
        }

        var message = string.Join(" ",
            results.Select(result => result.ErrorMessage).Where(error => error is not null));
        throw new RequestValidationException("request.validation_failed", message);
    }
}
