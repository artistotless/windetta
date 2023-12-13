using MassTransit.Events;
using MassTransit.QuartzIntegration;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Testing;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.TonTxns.Infrastructure.Sagas;
using Xunit.Abstractions;

namespace Windetta.TonTxnsTests.Sagas;

public class TonWithdrawFlowTests : IUseHarness
{
    private readonly XUnitOutWrapper _output;

    private const string _addr = "EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV";

    public TonWithdrawFlowTests(ITestOutputHelper output)
    {
        _output = new XUnitOutWrapper(output);
    }

    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddSagaStateMachine<TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        };
    }

    [Fact]
    public async Task WhenWithdrawRequested()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000uL
        };

        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();

        // act
        await harness.Bus.Publish<IWithdrawTonRequested>(argument);

        // assert
        (await harness.Consumed.Any<IWithdrawTonRequested>())
            .ShouldBe(true);
        (await sagaHarness.Consumed.Any<IWithdrawTonRequested>())
            .ShouldBe(true);
        (await sagaHarness.Exists(argument.CorrelationId, s => s.AwaitingDeduction))
            .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now()); ;
    }

    [Fact]
    public async Task WhenBalanceDeducted()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000uL
        };

        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        await harness.Bus.Publish<IWithdrawTonRequested>(argument);
        await harness.Consumed.Any<IWithdrawTonRequested>();
        (await harness.Sent.Any<IDeductBalance>()).ShouldBeTrue();

        // act
        await harness.Bus.Publish<IBalanceDeducted>(new BalanceDeductedImpl()
        {
            CorrelationId = argument.CorrelationId
        });

        // assert
        await sagaHarness.Consumed.Any<IBalanceDeducted>();
        // check schedule
        (await harness.Sent.Any<ISendTons>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(argument.CorrelationId, s => s.BalanceDeductedSuccess))
            .HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenBalanceDeductedFailed()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000uL
        };

        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        await harness.Bus.Publish<IWithdrawTonRequested>(argument);
        await harness.Consumed.Any<IWithdrawTonRequested>();
        var msg = harness.Sent.Select<IDeductBalance>()
            .FirstOrDefault()?.Context.Message;

        // act
        await harness.Bus.Publish(new FaultEvent<IDeductBalance>() { Message = msg });

        // assert
        await sagaHarness.Consumed.Any<Fault<IBalanceDeducted>>();
        (await sagaHarness.Exists(argument.CorrelationId, s => s.BalanceDeductFail))
            .HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenSendTonsCompleted()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000uL
        };

        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        await harness.Bus.Publish<IWithdrawTonRequested>(argument);
        await sagaHarness.Consumed.Any<IWithdrawTonRequested>();
        await harness.Bus.Publish<IBalanceDeducted>(new BalanceDeductedImpl()
        {
            CorrelationId = argument.CorrelationId
        });
        await sagaHarness.Consumed.Any<IBalanceDeducted>();
        await harness.Sent.Any<ISendTons>();

        // act
        await harness.Bus.Publish<ISendTonsCompleted>(new SendTonsCompletedImpl()
        {
            CorrelationId = argument.CorrelationId
        });

        // assert
        (await sagaHarness.Consumed.Any<ISendTonsCompleted>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(argument.CorrelationId, s => s.Final))
            .HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenSendTonsFailed()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000uL
        };

        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        await harness.Bus.Publish<IWithdrawTonRequested>(argument);
        await sagaHarness.Consumed.Any<IWithdrawTonRequested>();
        await harness.Bus.Publish<IBalanceDeducted>(new BalanceDeductedImpl()
        {
            CorrelationId = argument.CorrelationId
        });
        await sagaHarness.Consumed.Any<IBalanceDeducted>();
        var msg = harness.Sent.Select<ISendTons>()
            .FirstOrDefault()?.Context.Message;

        // act
        await harness.Bus.Publish(new FaultEvent<ISendTons>() { Message = msg });

        // assert
        (await sagaHarness.Consumed.Any<Fault<ISendTons>>())
            .ShouldBeTrue();
        (await harness.Sent.Any<IUnDeductBalance>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(argument.CorrelationId, s => s.SendTonsFail))
            .HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenBalanceUnDeductFailed()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000uL
        };

        await harness.Start();
        var stateMachineHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        await harness.Bus.Publish<IWithdrawTonRequested>(argument);
        await stateMachineHarness.Consumed.Any<IWithdrawTonRequested>();
        await harness.Bus.Publish<IBalanceDeducted>(new BalanceDeductedImpl()
        {
            CorrelationId = argument.CorrelationId
        });
        await stateMachineHarness.Consumed.Any<IBalanceDeducted>();
        var msgSendTons = harness.Sent.Select<ISendTons>()
            .FirstOrDefault()?.Context.Message;
        await harness.Bus.Publish(new FaultEvent<ISendTons>() { Message = msgSendTons });
        await stateMachineHarness.Consumed.Any<Fault<ISendTons>>();
        var msgBalanceUnDeducted = harness.Sent.Select<IUnDeductBalance>()
            .FirstOrDefault()?.Context.Message;

        // act
        await harness.Bus.Publish(new FaultEvent<IUnDeductBalance>()
        {
            Message = msgBalanceUnDeducted
        });

        // assert
        (await harness.Sent.Any<INotifyUnDeductBalanceFailed>()).ShouldBeTrue();
        (await stateMachineHarness.Exists(argument.CorrelationId,
             stateMachineHarness.StateMachine.BalanceUnDeductFail)).ShouldNotBeNull();
    }

    [Fact]
    public async Task WhenWithdrawalStatusRequested_ShouldReturnNotFound()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        var requestClient = harness.GetRequestClient<ITonWithdrawalStatusRequested>();

        // act
        var response = await requestClient.GetResponse<TonWithdrawalStatus, TonWithdrawalNotfound>(new
        {
            CorrelationId = Guid.NewGuid()
        });

        // assert
        response.Is(out Response<TonWithdrawalNotfound> _).ShouldBeTrue();
    }

    [Fact]
    public async Task WhenWithdrawalStatusRequested_ShouldReturnStatus()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000uL
        };
        await harness.Start();
        await harness.Bus.Publish<IWithdrawTonRequested>(argument);
        var stateMachineHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        await stateMachineHarness.Consumed.Any<IWithdrawTonRequested>();
        var requestClient = harness.GetRequestClient<ITonWithdrawalStatusRequested>();

        // act
        var response = await requestClient.GetResponse<TonWithdrawalStatus, TonWithdrawalNotfound>(new
        {
            CorrelationId = argument.CorrelationId
        });

        // assert
        response.Is(out Response<TonWithdrawalStatus> status).ShouldBeTrue();

        status.Message.destination.ShouldBe(argument.Destination);
        status.Message.failReason.ShouldBeNull();
        status.Message.nanotons.ShouldBe(argument.Nanotons);
    }


    [Fact]
    public async Task WhenSendTonsConfirmationPeriodExpired()
    {
        // arrange
        await using var provider = new ServiceCollection()
            .ConfigureMassTransit(Svc.TonTxns, this)
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress(_addr),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000uL
        };

        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        await harness.Bus.Publish<IWithdrawTonRequested>(argument);
        await sagaHarness.Consumed.Any<IWithdrawTonRequested>();
        await harness.Bus.Publish<IBalanceDeducted>(new BalanceDeductedImpl()
        {
            CorrelationId = argument.CorrelationId
        });

        await sagaHarness.Exists(argument.CorrelationId, x => x.BalanceDeductedSuccess);

        // act
        using var adjustment = new QuartzTimeAdjustment(provider);
        await adjustment.AdvanceTime(TimeSpan.FromMinutes(10));

        // assert
        (await sagaHarness.Exists(argument.CorrelationId, x => x.Expired))
            .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }
}

public class BalanceDeductedImpl : IBalanceDeducted
{
    public Guid CorrelationId { get; set; }
}

public class SendTonsCompletedImpl : ISendTonsCompleted
{
    public Guid CorrelationId { get; set; }
}