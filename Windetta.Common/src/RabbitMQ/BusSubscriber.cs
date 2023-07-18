using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Enrichers.MessageContext;
using System.Windows.Input;
using Windetta.Common.Handlers;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Common.RabbitMQ;

public class BusSubscriber : IBusSubscriber
{
    private readonly IServiceProvider _provider;
    private readonly RabbitMqOptions _options;
    private readonly IBusClient _busClient;
    private readonly ILogger _logger;

    public BusSubscriber(IApplicationBuilder builder)
    {
        _provider = builder.ApplicationServices.GetRequiredService<IServiceProvider>();
        _options = _provider.GetRequiredService<RabbitMqOptions>();
        _busClient = _provider.GetRequiredService<IBusClient>();
        _logger = _provider.GetRequiredService<ILogger<BusSubscriber>>();

        _options.Retries = _options.Retries <= 0 ? 3 : _options.Retries;
        _options.RetryInterval = _options.RetryInterval <= 0 ? 2 : _options.RetryInterval;
    }

    public IBusSubscriber SubscribeCommand<TCommand>(Func<TCommand, WindettaException, IRejectedEvent>? onError = null)
        where TCommand : ICommand
    {
        _busClient.SubscribeAsync<TCommand, CorrelationContext>(async (command, correlationContext) =>
        {
            var commandHandler = _provider.GetRequiredService<ICommandHandler<TCommand>>();

            return await TryHandleAsync(command, correlationContext,
                () => commandHandler.HandleAsync(command, correlationContext), onError);
        });

        return this;
    }

    public IBusSubscriber SubscribeEvent<TEvent>(string? @namespace = null, Func<TEvent, WindettaException, IRejectedEvent>? onError = null)
        where TEvent : IEvent
    {
        _busClient.SubscribeAsync<TEvent, CorrelationContext>(async (@event, correlationContext) =>
        {
            var eventHandler = _provider.GetRequiredService<IEventHandler<TEvent>>();

            return await TryHandleAsync(@event, correlationContext,
                () => eventHandler.HandleAsync(@event, correlationContext), onError);
        });

        return this;
    }

    // Internal retry for services that subscribe to the multiple events of the same types.
    private async Task<Acknowledgement> TryHandleAsync<TMessage>(TMessage message,
        CorrelationContext correlationContext, Func<Task> handle,
        Func<TMessage, WindettaException, IRejectedEvent>? onError = null)
    {
        var currentRetry = 0;
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(_options.Retries, i => TimeSpan.FromSeconds(_options.RetryInterval));

        var messageName = message.GetType().Name;

        return await retryPolicy.ExecuteAsync<Acknowledgement>(async () =>
        {
            // TODO: add tracer

            try
            {
                var retryMessage = currentRetry == 0 ? string.Empty : $"Retry: {currentRetry}'.";
                var preLogMessage = @$"Handling a message: '{messageName}' 
                with correlation id: '{correlationContext.Id}'. {retryMessage}";

                _logger.LogInformation(preLogMessage);

                await handle();

                var postLogMessage = @$"Handled a message: '{messageName}'
                with correlation id: '{correlationContext.Id}'. {retryMessage}";

                _logger.LogInformation(postLogMessage);

                return new Ack();
            }
            catch (Exception exception)
            {
                currentRetry++;

                _logger.LogError(exception, exception.Message);

                if (exception is WindettaException windettaException && onError != null)
                {
                    var rejectedEvent = onError(message, windettaException);

                    await _busClient.PublishAsync(rejectedEvent, ctx => ctx.UseMessageContext(correlationContext));

                    _logger.LogInformation($"Published a rejected event: '{rejectedEvent.GetType().Name}' " +
                                           $"for the message: '{messageName}' with correlation id: '{correlationContext.Id}'.");
                    return new Ack();
                }

                throw new Exception(@$"Unable to handle a message: '{messageName}' 
                                    with correlation id: '{correlationContext.Id}',
                                    retry {currentRetry - 1}/{_options.Retries}...");
            }
        });
    }
}
