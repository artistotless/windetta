using Windetta.Common.Types;

namespace Windetta.TonTxns.Application.DAL;

public interface IDepositsRepository : IScopedService
{
    public Task<Guid> GetLastId();
    Task UpdateLastId(Guid lastId);
}