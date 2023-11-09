namespace Windetta.TonTxns.Application.Services.Audit;

public interface IDepositsHistory
{
    Task<ulong> GetLastLtAsync();
    Task UpdateLastLtAsync(ulong lastLt);
}