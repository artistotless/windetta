using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services.Audit;

namespace Windetta.TonTxns.Application.Services;

public class DepositFacade
{
    public event EventHandler<FundsAddedEventArg>? FundsAdded;

    private readonly IDepositsHistory _history;
    private readonly ITransactionsLoader _loader;
    private readonly ITransactionsMapper _mapper;

    public DepositFacade(
        IDepositsHistory history,
        ITransactionsLoader loader,
        ITransactionsMapper mapper)
    {
        _history = history;
        _loader = loader;
        _mapper = mapper;
    }

    public async void Process()
    {
        var lastLt = await _history.GetLastLtAsync();
        var receipts = await _loader.LoadAsync(lastLt);
        var fundsAddedData = _mapper.Map(receipts);

        lastLt = receipts.Max(x => x.Lt);
        await _history.UpdateLastLtAsync(lastLt);

        FundsAdded?.Invoke(this, new FundsAddedEventArg(fundsAddedData));
    }
}
