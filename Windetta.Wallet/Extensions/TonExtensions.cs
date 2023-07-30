using TonLibDotNet.Types;
using Windetta.Wallet.Infrastructure.Models;

namespace Windetta.Wallet.Extensions;

public static class TonExtensions
{
    public static long Summarize(this TonLibDotNet.Types.Query.Fees fees)
    {
        return fees.SourceFees.FwdFee + fees.SourceFees.InFwdFee + fees.SourceFees.GasFee + fees.SourceFees.StorageFee + fees.DestinationFees.Sum(x => x.InFwdFee + x.FwdFee + x.GasFee + x.StorageFee);
    }

    public static Key ToKey(this TonWalletCredential credential)
        => new Key() { PublicKey = credential.PublicKey, Secret = credential.SecretKey };

}
