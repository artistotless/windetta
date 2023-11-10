namespace Windetta.TonTxns.Application.Services.Audit;

public interface IDepositsHistory
{
    Task<ulong> GetLastIdAsync();
    Task UpdateLasIdAsync(ulong lastId);
}