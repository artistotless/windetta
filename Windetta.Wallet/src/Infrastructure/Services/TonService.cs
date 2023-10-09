using TonLibDotNet;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Msg;
using TonLibDotNet.Types.Query;
using TonLibDotNet.Types.Wallet;
using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Wallet.Extensions;
using Windetta.Wallet.Infrastructure.Models;

namespace Windetta.Wallet.Infrastructure.Services;

public class TonService : ITonService
{
    private readonly ITonClient _tonClient;
    private readonly ILogger _logger;

    public TonService(ITonClient tonClient, ILogger<TonService> logger)
    {
        _tonClient = tonClient;
        _logger = logger;
    }

    public async Task<TonWallet> GenerateWalletData()
    {
        await InitTonClient();

        // Get the default wallet ID from the TonClient's configuration
        var walletId = _tonClient.OptionsInfo.ConfigInfo.DefaultWalletId;

        // Create a new cryptographic key pair
        var key = await _tonClient.CreateNewKey();

        _logger.LogInformation("New key: public = {PublicKey}, secret = {Secret}", key.PublicKey, key.Secret);

        // Create the initial account state for the wallet using the new public key and the wallet ID
        var initialAccountState = new V3InitialAccountState() { PublicKey = key.PublicKey, WalletId = walletId };

        // Obtain the account address based on the initial account state
        var address = await _tonClient.GetAccountAddress(initialAccountState, 0, 0);

        var keys = new TonWalletCredential()
        {
            PublicKey = key.PublicKey,
            SecretKey = key.Secret,
        };

        var tonWallet = new TonWallet()
        {
            Address = new(address.Value),
            Credential = keys,
        };

        return tonWallet;
    }

    /// <summary>
    // This method estimates the transaction fees for a transfer from one TON wallet to another.
    // It takes the sender's wallet credentials, the recipient's address ('to'), and the amount in nanotons to transfer.
    // The estimated fees are returned as a long integer.
    /// </summary>
    public async Task<long> EstimateFees(TonWalletCredential from, string to, long nanotonAmount)
    {
        // Build the query for the transfer using the provided sender's wallet credentials, recipient address, and amount.
        var query = await BuildTransferQuery(from, to, nanotonAmount);

        // Call the TON Client to estimate the transaction fees based on the constructed query.
        TonLibDotNet.Types.Query.Fees fees = await _tonClient.QueryEstimateFees(query.Id);

        // Summarize and return the estimated transaction fees as a long integer.
        // The 'Summarize()' method is assumed to calculate the total fees from the 'fees' object.
        return fees.Summarize();
    }

    /// <summary>
    // This method retrieves the balance of a specified account using the TON Client library.
    // It returns the account balance as a long integer representing the amount in nanotons.
    /// </summary>
    public async Task<long> GetBalance(string address)
    {
        // Ensure the TON Client is initialized before making any requests.
        await InitTonClient();

        // Call the TON Client to get the account state for the given address.
        var ast = await _tonClient.GetAccountState(address);

        // Log the balance information for debugging purposes.
        // The 'ast.Balance' value represents the account balance in nanotons.
        // The second placeholder is for the balance converted to TON using the 'TonUtils.Coins.FromNano' method.
        _logger.LogInformation("balance = {Value} nanoton or {Value} TON", ast.Balance, TonUtils.Coins.FromNano(ast.Balance));

        // Return the account balance as a long integer in nanotons.
        return ast.Balance;
    }

    /// <summary>
    // This method transfers a specified amount of TONs from one wallet to another.
    // It takes the sender's wallet credentials, the recipient's address ('to'), and the amount in nanotons to transfer.
    // The method returns a TransferInfo object with details about the transfer, including the actual amount transferred, the recipient address, and the total transaction fees.
    /// </summary>
    public async Task<TransferResult> TransferTon(TonWalletCredential from, string to, long nanotonAmount)
    {
        // Estimate the transaction fees for the transfer using the provided sender's wallet credentials, recipient address, and amount.
        var sumFee = await EstimateFees(from, to, nanotonAmount);

        // Calculate the actual amount to be transferred after deducting the fees.
        var amount = Math.Max(0, nanotonAmount - sumFee);

        // Check if the amount to transfer after deducting the fees is positive (non-negative).
        // If the amount is non-positive, it means the fees are greater than or equal to the transfer amount, which is not allowed.
        // In such a case, throw a WindettaException with a custom error message indicating the issue.
        if (amount <= 0)
            throw new WindettaException(Errors.Wallet.BigFeeTransaction, "The fee is more than the balance");

        // Build the query for the transfer using the provided sender's wallet credentials, recipient address, and adjusted amount.
        var query = await BuildTransferQuery(from, to, nanotonAmount - sumFee);

        // Call the TON Client to send the transfer request based on the constructed query.
        // The result of the send operation is ignored (using "_ =") as it's not used in this code snippet.
        _ = await _tonClient.QuerySend(query.Id);

        // Create a TransferInfo object to store details about the transfer.
        // This includes the actual amount transferred ('amount'), the recipient address ('to'), and the total transaction fees ('sumFee').
        return new TransferResult()
        {
            Amount = amount,
            Destination = to,
            TotalFee = sumFee,
        };
    }

    private async Task InitTonClient()
    {
        // Ensure the TonClient is initialized and ready for use
        await _tonClient.InitIfNeeded();
    }

    public async Task<Info> BuildTransferQuery(TonWalletCredential from, string to, long nanotonAmount)
    {
        await InitTonClient();

        var walletId = _tonClient.OptionsInfo.ConfigInfo.DefaultWalletId;
        var initialAccountState = new V3InitialAccountState()
        {
            PublicKey = from.PublicKey,
            WalletId = walletId
        };

        var address = await _tonClient.GetAccountAddress(initialAccountState, 0, 0);
        var msg = new Message(new AccountAddress(to))
        {
            Data = new DataText(DataText.Int256_AllZeroes),
            Amount = nanotonAmount,
            SendMode = 1,
        };

        var action = new ActionMsg(new[] { msg }.ToList()) { AllowSendToUninited = true };

        var query = await _tonClient.CreateQuery
            (new InputKeyRegular(from.ToKey()), address, action,
            TimeSpan.FromMinutes(1),
            initialAccountState: initialAccountState);

        return query;
    }
}
