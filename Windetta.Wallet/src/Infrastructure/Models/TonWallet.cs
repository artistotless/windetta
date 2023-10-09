using Windetta.Common.Types;

namespace Windetta.Wallet.Infrastructure.Models;

public class TonWallet
{
    public TonAddress Address { get; set; }
    public TonWalletCredential Credential { get; set; }
}
