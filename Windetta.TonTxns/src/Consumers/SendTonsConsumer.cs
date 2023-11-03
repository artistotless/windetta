using MassTransit;
using MassTransit.Events;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.TonTxns.Application.Services;
using Windetta.TonTxns.Infrastructure.Models;
using Windetta.TonTxns.Infrastructure.Services;

namespace Windetta.TonTxns.Consumers;

public class SendTonsConsumer : IConsumer<IPackedSendTons>
{
    public const int RetryCount = 3;

    private readonly ITonService _tonService;
    private readonly IWalletCredentialSource _credentialSource;
    private readonly ITransactionsService _txnService;

    public SendTonsConsumer(
        ITonService tonService,
        IWalletCredentialSource credentialSource,
        ITransactionsService txnService)
    {
        _tonService = tonService;
        _credentialSource = credentialSource;
        _txnService = txnService;
    }

    public async Task Consume(ConsumeContext<IPackedSendTons> context)
    {
        if (await _txnService.ExistAsync(context.Message.CorrelationId))
            return;

        var transferMessages = LoadTransferMessageArray(context.Message.Sends);
        var totalAmount = transferMessages.Sum(x => x.nanotons);

        await _txnService.AddAsync(new()
        {
            Id = context.Message.CorrelationId,
            Status = Domain.TransactionStatus.Pending,
            TotalAmount = totalAmount,
            TransfersCount = transferMessages.Count()
        });

        var credential = await _credentialSource.GetCredential();

        try
        {
            await _tonService.SendTons(credential, transferMessages);
        }
        catch (Exception ex)
        {
            if (context.GetRetryCount() >= RetryCount)
                await context.PublishBatch(BuildFaultMessages(context, ex));
            else
                throw new SendTonsException(ex.Message);
        }

        await context.PublishBatch(BuildConfirmationMessages(context.Message.Sends));
    }

    private IEnumerable<ISendTonsCompleted> BuildConfirmationMessages(ISendTons[] sends)
    {
        foreach (var item in sends)
            yield return new SendTonsCompleted(item.CorrelationId);
    }

    private IEnumerable<FaultEvent<ISendTons>> BuildFaultMessages(ConsumeContext<IPackedSendTons> context, Exception e)
    {
        var messageId = context.MessageId;
        var host = context.Host;

        string[]? supportedTypes = null;

        if (context.SupportedMessageTypes.Count() > 0)
            supportedTypes = context.SupportedMessageTypes.ToArray();

        foreach (var item in context.Message.Sends)
            yield return new FaultEvent<ISendTons>(item, messageId, host, e, supportedTypes);
    }

    private IEnumerable<TransferMessage> LoadTransferMessageArray(ISendTons[] array)
    {
        foreach (var item in array)
            yield return new TransferMessage(item.Destination, item.Nanotons);
    }
}

public record SendTonsCompleted(Guid CorrelationId) : ISendTonsCompleted;

public class SendTonsConsumerDefinition : ConsumerDefinition<SendTonsConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<SendTonsConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Handle<SendTonsException>();
            r.Interval(retryCount: SendTonsConsumer.RetryCount, interval: TimeSpan.FromSeconds(5));
        });
    }
}