using Windetta.Common.Types;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.DAL;

public interface IWallets : IScopedService
{
    public void Add(UserWallet wallet);
    public Task<UserWallet?> GetAsync(Guid userId);
    public Task<IEnumerable<UserWallet>> GetAllAsync(IEnumerable<Guid> userIds);
}