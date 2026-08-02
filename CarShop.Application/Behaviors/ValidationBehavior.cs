using FluentValidation;
using MediatR;

namespace CarShop.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count == 0)
                return await next();

            var errors = failures.Select(f => f.ErrorMessage).ToList();

            // TResponse is always Result<T> for our commands/queries; reflection lets one
            // behavior build a Result<T>.Fail(...) for any T without a generic constraint on Result<T>.
            var failMethod = typeof(TResponse).GetMethod("Fail", new[] { typeof(List<string>), typeof(string) });
            if (failMethod != null)
                return (TResponse)failMethod.Invoke(null, new object?[] { errors, null })!;

            throw new ValidationException(failures);
        }
    }
}
