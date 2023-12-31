﻿using MassTransit;
using MassTransit.Events;
using Polly;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;
using Windetta.TonTxns.Application.Services.Audit;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Infrastructure.Consumers;

public class SendTonsConsumer : IConsumer<IPackedSendTons>
{
    public const int RetryCount = 3;

    private readonly IWithdrawService _tonService;
    private readonly IWalletCredentialSource _credentialSource;
    private readonly IWithdrawals _txnService;

    public SendTonsConsumer(
        IWithdrawService tonService,
        IWalletCredentialSource credentialSource,
        IWithdrawals txnService)
    {
        _tonService = tonService;
        _credentialSource = credentialSource;
        _txnService = txnService;
    }

    public async Task Consume(ConsumeContext<IPackedSendTons> context)
    {
        // TODO: If txn.Status == pending - Search ton transaction by bodyHash,
        // which should be precalculated here
        if (await _txnService.ExistAsync(context.Message.CorrelationId))
            return;

        var transferMessages = LoadTransferMessageArray(context.Message.Sends);
        var totalAmount = (ulong)transferMessages.Sum(x => (long)x.nanotons);

        var txn = new Withdrawal()
        {
            Id = context.Message.CorrelationId,
            Status = WithdrawalStatus.Pending,
            TotalAmount = totalAmount,
            TransfersCount = transferMessages.Count()
        };

        await GetRetryPipeline().ExecuteAsync(async (token) =>
        {
            await _txnService.AddAsync(txn);
        });

        try
        {
            await GetRetryPipeline().ExecuteAsync(async (token) =>
            {
                await _tonService.ExecuteWithdraw(
                    _credentialSource.Value, transferMessages);
            });
        }
        catch (Exception ex)
        {
            await context.PublishBatch(
                BuildFaultMessages(context, ex));

            return;
        }

        await context.PublishBatch(
            BuildConfirmationMessages(context.Message.Sends));

        await GetRetryPipeline().ExecuteAsync(async (token) =>
        {
            txn.Status = WithdrawalStatus.Confirmed;
            await _txnService.UpdateAsync(txn);
        });
    }

    private static ResiliencePipeline GetRetryPipeline()
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new()
            {
                MaxRetryAttempts = RetryCount,
                Delay = TimeSpan.FromSeconds(5)
            }).Build();
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