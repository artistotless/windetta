using Microsoft.Extensions.Options;
using TonSdk.Client;
using TonSdk.Contracts.Wallet;
using TonSdk.Core.Block;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;
using Windetta.TonTxns.Infrastructure.Extensions;

namespace Windetta.TonTxns.Infrastructure.Services;

public class HttpTonService : IWithdrawService
{
    private readonly TonClient _client;

    public HttpTonService(IOptions<TonClientParameters> parameters)
    {
        _client = new TonClient(parameters.Value);
    }

    public async Task ExecuteWithdraw(WalletCredential from, IEnumerable<TransferMessage> messages)
    {
        // Create a new preprocessed wallet using the public key
        var wallet = new HighloadV2(new HighloadV2Options
        {
            PublicKey = Convert.FromBase64String(from.PublicKey),
            SubwalletId = 698983191,
            Workchain = 0
        });

        // Create a transfer message for the wallet
        ExternalInMessage message = wallet.CreateTransferMessage(
            messages.ToWalletTransfers().ToArray())
           .Sign(Convert.FromBase64String(from.PrivateKey));

        // Send the serialized message
        await _client.SendBoc(message.Cell!);
    }
}
