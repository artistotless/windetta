﻿using Windetta.Common.Types;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services.Audit;

namespace Windetta.TonTxns.Application.Services;

public class DepositPoller : IScopedService
{
    private readonly IDepositsHistory _history;
    private readonly ITransactionsLoader _loader;
    private readonly ITransactionMapper _mapper;

    public DepositPoller(
        IDepositsHistory history,
        ITransactionsLoader loader,
        ITransactionMapper mapper)
    {
        _history = history;
        _loader = loader;
        _mapper = mapper;
    }

    public async Task<DepositPollResult> ProcessAsync()
    {
        var lastId = await _history.GetLastIdAsync();
        var transactions = await _loader.LoadAsync(lastId);

        if (transactions is null || transactions.Count() <= 0)
            return new DepositPollResult(new List<FundsFoundData>());

        var fundsAddedData = transactions.Select(_mapper.Map);

        lastId = transactions.Last().Id;
        await _history.UpdateLasIdAsync(lastId);

        return new DepositPollResult(fundsAddedData);
    }
}
