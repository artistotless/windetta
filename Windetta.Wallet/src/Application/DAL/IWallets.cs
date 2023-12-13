using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.DAL;

public interface IWallets
{
    public Task AddAsync(UserWallet wallet);
    public Task<UserWallet?> GetAsync(Guid userId);
}