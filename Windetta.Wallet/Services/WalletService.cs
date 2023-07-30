using TonLibDotNet;
using TonLibDotNet.Types.Wallet;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Services;

public class WalletService : IWalletService
{
    private readonly ITonClient _tonClient;
    //private readonly IWalletsRepository _rep;
    //private readonly IWalletCredentialRepository _rep;

    public WalletService(ITonClient tonClient)
    {
        _tonClient = tonClient;
    }

    public Task<UserWallet> CreateWallet(Guid userId)
    {
        var (addr, credentials) = await CreateWalletCredentials(@event.Id);
        var userWallet = new UserWallet()
        {
            UserId = @event.Id,
            Address = addr,
            HeldBalance = 0
        };

        await Task.CompletedTask;
    }

    private async Task<(string, WalletCredential)> CreateWalletCredentials(Guid userId)
    {
        // Ensure the TonClient is initialized and ready for use
        await _tonClient.InitIfNeeded();

        // Get the default wallet ID from the TonClient's configuration
        var walletId = _tonClient.OptionsInfo.ConfigInfo.DefaultWalletId;

        // Create a new cryptographic key pair
        var key = await _tonClient.CreateNewKey();
        _logger.LogInformation("New key: public = {PublicKey}, secret = {Secret}", key.PublicKey, key.Secret);

        // Create the initial account state for the wallet using the new public key and the wallet ID
        var initialAccountState = new V3InitialAccountState() { PublicKey = key.PublicKey, WalletId = walletId };

        // Obtain the account address based on the initial account state
        var address = await _tonClient.GetAccountAddress(initialAccountState, 0, 0);

        // Return the generated account address and the corresponding wallet credentials as a tuple
        return (address.Value, new WalletCredential()
        {
            // Store the private and public keys along with the user ID and a flag indicating if the PIN code is set
            PrivateKey = key.Secret,
            PublicKey = key.PublicKey,
            UserId = userId,
        });
    }

    public Task<Balance> GetBalance(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetDepositAddress(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task HoldBalance(Guid userId, int nanotonAmount)
    {
        throw new NotImplementedException();
    }

    public Task UnHoldBalance(Guid userId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Transfers specified amount of nanotons from user's balance
    /// </summary>
    /// <param name="from">Source wallet credential</param>
    /// <param name="to">Destination TON address</param>
    /// <param name="nanotonAmount">amount of nanotons</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task TransferTon(WalletCredential from, string to, int nanotonAmount)
    {
        throw new NotImplementedException();
    }
}
