using Windetta.Contracts.Events;

namespace Windetta.TonTxns.Application.Models;

public class FundsAddedEventArg : EventArgs
{
    public IEnumerable<FundsAddedData> Values;

    public FundsAddedEventArg(IEnumerable<FundsAddedData> values)
    {
        Values = values;
    }
}

public class FundsAddedData : IFundsAdded
{
    public Guid UserId { get; set; }
    public long Amount { get; set; }
    public int CurrencyId { get; set; }
    public Guid CorrelationId { get; set; }
}