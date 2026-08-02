using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CarShop.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();
                stopwatch.Stop();

                // Most requests return Result<T>; reflection reads .Success without a generic
                // constraint on Result<T> (same trick ValidationBehavior uses for .Fail(...)).
                var success = typeof(TResponse).GetProperty("Success")?.GetValue(response) as bool?;
                if (success == false)
                    _logger.LogWarning("{RequestName} failed in {ElapsedMilliseconds}ms", requestName, stopwatch.ElapsedMilliseconds);
                else
                    _logger.LogInformation("{RequestName} completed in {ElapsedMilliseconds}ms", requestName, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "{RequestName} threw after {ElapsedMilliseconds}ms", requestName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
