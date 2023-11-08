using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.DAL;

public interface IWalletsRepository
{
    public Task AddAsync(UserWallet wallet);
    public Task<UserWallet?> GetAsync(Guid userId);
}