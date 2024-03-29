﻿using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Exceptions;

namespace Windetta.Wallet.Infrastructure.Consumers;
public class CreateConsumer : IConsumer<ICreateUserWallet>
{
    private readonly IUserWalletService _walletService;

    public CreateConsumer(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    public async Task Consume(ConsumeContext<ICreateUserWallet> context)
    {
        await _walletService.CreateWalletAsync(context.Message.UserId);

        await context.Publish<IUserWalletCreated>(new
        {
            context.Message.UserId,
            TimeStamp = DateTime.UtcNow,
        });
    }
}

public class CreateConsumerDefinition : ConsumerDefinition<CreateConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<CreateConsumer> consumerConfigurator, IRegistrationContext context)
    {
        consumerConfigurator.UseScheduledRedelivery(r =>
        {
            r.Ignore<WalletException>();
            r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(4), TimeSpan.FromMinutes(10));
        });

        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Ignore<WalletException>();
            r.Interval(retryCount: 3, interval: TimeSpan.FromSeconds(10));
        });
    }
}
