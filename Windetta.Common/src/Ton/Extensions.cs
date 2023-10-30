using TonSdk.Core.Crypto;

namespace Windetta.Common.Ton;

public static class Extensions
{
    public static (string @public, string @private) GetKeysPair(this Mnemonic mnemonic)
    => (Convert.ToBase64String(mnemonic.Keys.PublicKey), Convert.ToBase64String(mnemonic.Keys.PrivateKey));

    public static (string @public, string @private) GetKeysPair(this MnemonicBIP39 mnemonic)
        => (Convert.ToBase64String(mnemonic.Keys.PublicKey), Convert.ToBase64String(mnemonic.Keys.PrivateKey));
}
