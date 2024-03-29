﻿using MassTransit;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;
using Windetta.Main.Core.Services.Wallet;

namespace Windetta.Main.Infrastructure.Services;

[AutoInjectExclude]
public class AsyncWalletService : IWalletService
{
    private readonly IRequestClient<IBalanceRequested> _client;

    public AsyncWalletService(IRequestClient<IBalanceRequested> client)
    {
        _client = client;
    }

    public async Task<UserBalanceResponse> GetBalance(Guid userId, int currencyId)
    {
        var response = await _client.GetResponse<UserBalanceResponse>(new
        {
            UserId = userId,
            CurrencyId = currencyId,
        });

        return response.Message;
    }

    public async Task<bool> IsEqualOrGreater(Guid userId, FundsInfo funds)
    {
        var response = await _client.GetResponseOrThrow<IBalanceRequested, UserBalanceResponse>(new
        {
            UserId = userId,
            funds.CurrencyId,
        });

        return response.AvailableAmount >= funds.Amount;
    }
}
