using Windetta.Common.Types;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Application.DAL;

[AutoInjectExclude]
public class InMemoryDepositsRepository : IDepositsRepository
{
    private readonly List<Deposit> _storage;

    public InMemoryDepositsRepository()
        => _storage = new List<Deposit>();

    public Task<Guid> GetLastId()
        => Task.FromResult(_storage.LastOrDefault()?.Id ?? Guid.Empty);

    public Task UpdateLastId(Guid lastId)
    {
        _storage.Add(new Deposit()
        {
            Created = DateTimeOffset.UtcNow,
            Id = lastId
        });

        return Task.CompletedTask;
    }
}