using Microsoft.Extensions.Options;
using TonSdk.Client;
using TonSdk.Contracts.Wallet;
using TonSdk.Core;
using TonSdk.Core.Block;
using TonSdk.Core.Boc;
using TonSdk.Core.Crypto;
using Windetta.Common.Types;
using Windetta.Wallet.Extensions;
using Windetta.Wallet.Infrastructure.Models;

namespace Windetta.Wallet.Infrastructure.Services;

public class HttpTonService : ITonService
{
    private readonly TonClient _client;

    public HttpTonService(IOptions<TonClientParameters> parameters)
    {
        _client = new TonClient(parameters.Value);
    }

    public ValueTask<TonWallet> GenerateWalletData()
    {
        Mnemonic mnemonic = new Mnemonic();
        // Create a new preprocessed wallet using the public key from the generated mnemonic

        WalletV3 wallet = new WalletV3(new WalletV3Options
        {
            PublicKey = mnemonic.Keys.PublicKey!
        });

        // Convert the address to a non-bounceable format
        string nonBounceableAddress = wallet.Address.ToString(
            AddressType.Base64, new AddressStringifyOptions(false, false, true));

        (string publicKey, string privateKey) = mnemonic.GetKeysPair();

        return ValueTask.FromResult(new TonWallet()
        {
            Address = new TonAddress(nonBounceableAddress),
            Credential = new TonWalletCredential()
            {
                PublicKey = publicKey,
                PrivateKey = privateKey,
            },
        });
    }

    public Task<long> EstimateFees(TonWalletCredential from, string to, long nanotons)
    {
        throw new NotImplementedException();
    }

    public async Task<long> GetBalance(string address)
    {
        Coins balance = await _client.GetBalance(new(address));

        return long.Parse(balance.ToNano());
    }

    public async Task<TransferResult> TransferTon(TonWalletCredential from, string to, long nanotons)
    {
        // Create a new preprocessed wallet using the public key
        WalletV3 wallet = new WalletV3(new WalletV3Options
        {
            PublicKey = Convert.FromBase64String(from.PublicKey),
        });

        // Retrieve the wallet data
        var walletData = (await _client.GetAddressInformation(wallet.Address)).Data;

        // Extract the sequence number from the wallet data, or set it to 0 if the data is null
        var seqno = walletData == null ? 0 : wallet.ParseStorage(walletData.Parse()).Seqno;

        // Create a transfer message for the wallet
        ExternalInMessage message = wallet.CreateTransferMessage(new[]{
            CreateTransferMessage(to,nanotons)}, seqno)
           .Sign(Convert.FromBase64String(from.PrivateKey));

        // Send the serialized message
        await _client.SendBoc(message.Cell!);

        return new TransferResult()
        {
            Amount = 0,
        };
    }

    private WalletTransfer CreateTransferMessage(string destination, long nanotons)
    {
        return new WalletTransfer
        {
            Message = new InternalMessage(new()
            {
                Info = new IntMsgInfo(new()
                {
                    Dest = new Address(destination),
                    Value = Coins.FromNano(nanotons)
                }),
                Body = new CellBuilder()
                .StoreUInt(0, 32)
                .StoreString(string.Empty)
                .Build(),
            }),
            Mode = 1
        };
    }
}
