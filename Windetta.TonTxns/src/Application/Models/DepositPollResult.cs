namespace Windetta.TonTxns.Application.Models;

public class DepositPollResult
{
    public IEnumerable<FundsFoundData> Values;

    public DepositPollResult(IEnumerable<FundsFoundData> values)
    {
        Values = values;
    }
}

public class FundsFoundData
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public long Amount { get; set; }
}