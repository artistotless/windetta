using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Testing;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Identity.Domain.Entities;
using Windetta.Identity.Infrastructure.Sagas;
using Xunit.Abstractions;

namespace Windetta.IdentityTests.SagasTests;

public class NewUserFlowTests : IUseHarness
{
    private readonly XUnitOutWrapper _output;

    private const string email = "user@windetta.com";
    private const string username = "windettaUser";


    public NewUserFlowTests(ITestOutputHelper output)
    {
        _output = new XUnitOutWrapper(output);
    }

    #region Configuration
    private ServiceProvider GetProvider()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.ConfigureTestMassTransit(Svc.Main, this);

        return serviceCollection.BuildServiceProvider();
    }

    private NewUserFlow CreateSagaWithState(NewUserFlowState initialState)
    {
        return new NewUserFlow
        {
            CurrentState = (int)initialState,
            CorrelationId = ExampleGuids.UserId,
            Email = email,
            UserId = ExampleGuids.UserId
        };
    }

    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddSagaStateMachine<NewUserFlowStateMachine, NewUserFlow>();
        };
    }
    #endregion

    [Fact]
    public async Task When_UserCreated()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();
        var argument = new
        {
            CorrelationId = ExampleGuids.UserId,
            TimeStamp = DateTimeOffset.UtcNow,
            Id = ExampleGuids.UserId,
            UserName = username,
            Email = email,
            Role = Roles.USER,
        };
        var sagaHarness = harness.GetSagaStateMachineHarness
            <NewUserFlowStateMachine, NewUserFlow>();

        // act
        await harness.Bus.Publish<IUserCreated>(argument);

        // assert
        (await sagaHarness.Consumed.Any<IUserCreated>())
                .ShouldBeTrue();
        (await sagaHarness.Exists(argument.CorrelationId, x => x.AwaitingConfirmation))
                .HasValue.ShouldBeTrue();
        (await harness.Sent.Any<INotifyEmailConfirmation>())
                        .ShouldBeTrue();
        (await harness.Sent.Any<INotifyUserCreated>())
                        .ShouldBeTrue();
        (await harness.Sent.Any<ICreateUserWallet>())
                        .ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_EmailConfirmed()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();
        var correllationId = ExampleGuids.UserId;

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (NewUserFlowState.AwaitingConfirmation));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <NewUserFlowStateMachine, NewUserFlow>();

        // act
        await harness.Bus.Publish<IUserEmailConfirmed>(new
        {
            ExampleGuids.UserId
        });
        await sagaHarness.Consumed.Any<IUserEmailConfirmed>();

        // assert
        (await sagaHarness.NotExists(correllationId))
            .HasValue.ShouldBeFalse();

        await harness.OutputTimeline(_output, x => x.Now());
    }

}