using MediatR;
using System.Text.Json;

namespace Acceloka.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var correlationId = Guid.NewGuid();

            var requestJson = JsonSerializer.Serialize(request);
            
            logger.LogInformation($"Handling request {correlationId}: {requestJson}");

            var response = await next();

            var responseJson = JsonSerializer.Serialize(response);

            logger.LogInformation($"Response for {correlationId}: {responseJson}");

            return response;
        }
    }
}
