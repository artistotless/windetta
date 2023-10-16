using TonSdk.Core.Crypto;

namespace Windetta.Wallet.Extensions;

public static class TonExtensions
{
    public static (string @public, string @private) GetKeysPair(this Mnemonic mnemonic)
        => (Convert.ToBase64String(mnemonic.Keys.PublicKey), Convert.ToBase64String(mnemonic.Keys.PrivateKey));
}
