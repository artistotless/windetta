using TonSdk.Contracts.Wallet;
using TonSdk.Core;
using TonSdk.Core.Block;
using Windetta.TonTxns.Infrastructure.Models;

namespace Windetta.TonTxns.Extensions;

public static class Extensions
{
    public static WalletTransfer ToWalletTransfer(this TransferMessage msg)
    {
        return new WalletTransfer
        {
            Message = new InternalMessage(new()
            {
                Info = new IntMsgInfo(new()
                {
                    Dest = new Address(msg.destination),
                    Value = Coins.FromNano(msg.nanotons)
                }),
            }),
            Mode = 64
        };
    }

    public static IEnumerable<WalletTransfer> ToWalletTransfers(this IEnumerable<TransferMessage> msg)
    {
        foreach (var item in msg)
            yield return item.ToWalletTransfer();
    }
}
