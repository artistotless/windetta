using Windetta.TonTxns.Infrastructure.Models;

namespace Windetta.TonTxnsTests;

public static class Extensions
{
    public static IEnumerable<TransferMessage> ToTransferMessages(this IEnumerable<TransferTonImpl> array)
    {
        return array.Select(x => new TransferMessage(x.Destination, x.Nanotons));
    }
}
