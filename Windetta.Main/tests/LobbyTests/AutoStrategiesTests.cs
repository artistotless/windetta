﻿using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Testing;
using Windetta.Contracts;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.MainTests.Mocks;
using FullRoomsReadyStrategy = Windetta.MainTests.Mocks.FullRoomsReadyStrategy;

namespace Windetta.MainTests.LobbyTest;

public class AutoStrategiesTests
{
    [Fact]
    public async Task TestFullRoomsReadyStrategy()
    {
        // arrange
        var request = new CreateLobbyDto()
        {
            Bet = new FundsInfo(1, 1000),
            AutoReadyStrategy = new PluginSetDto(nameof(FullRoomsReadyStrategy)),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<LobbiesInteractor>();

        ILobby lobby = await interactor.CreateAsync(request);

        var member2Id = Guid.NewGuid();

        bool autoStrategyWorkedOut = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            autoStrategyWorkedOut = true;
        };

        lobby.Ready += callback;

        // act
        await interactor.JoinMemberAsync(member2Id, lobby.Id, roomIndex: 0);

        await tcs.Task;

        // assert
        autoStrategyWorkedOut.ShouldBeTrue();
    }

    [Fact]
    public async Task TestAutoDisposeStrategy()
    {
        // arrange
        var request = new CreateLobbyDto()
        {
            Bet = new FundsInfo(1, 800),
            AutoDisposeStrategy = new PluginSetDto(nameof(DateDisposeStrategy)),
            GameId = ExampleGuids.GameId,
            InitiatorId = ExampleGuids.UserId
        };

        var interactor = SharedServiceProvider.GetInstance()
            .GetRequiredService<LobbiesInteractor>();

        ILobby lobby = await interactor.CreateAsync(request);
        bool autoStrategyWorkedOut = false;
        var tcs = new TaskCompletionSource<bool>();

        EventHandler callback = delegate (object? sender, EventArgs e)
        {
            tcs.SetResult(true);
            autoStrategyWorkedOut = true;
        };


        lobby.Disposed += callback;
        await interactor.LeaveMemberAsync(request.InitiatorId, lobby.Id);

        // act
        await tcs.Task;

        // assert
        autoStrategyWorkedOut.ShouldBeTrue();
    }
}
