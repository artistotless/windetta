using Microsoft.Extensions.Options;
using TonSdk.Client;
using Windetta.Common.Ton;
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
        // Create a new wallet using the public key
        var wallet = new HighloadV1Custom(new()
        {
            PublicKey = Convert.FromBase64String(from.PublicKey),
            SubwalletId = 698983191,
            Workchain = 0
        });

        // Get seqno from smart contract
        var seqno = await _client.Wallet.GetSeqno(wallet.Address) ?? 0;

        // Create a transfer message for the wallet
        var message = wallet.CreateTransferMessage(
            messages.ToWalletTransfers().ToArray(),
            seqno)
           .Sign(Convert.FromBase64String(from.PrivateKey));

        // Send the serialized message
        await _client.SendBoc(message.Cell!);
    }
}
