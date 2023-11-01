using Windetta.Common.Types;

namespace Windetta.Wallet.Application.Services;

public class DefaultDepositAddressSource : IDepositAddressSource
{
    public ValueTask<TonAddress> GetAddressAsync()
    {
        return ValueTask.FromResult(
            new TonAddress("EQDr3kCpf0zPVWR79VrTt-bsbIa-ND9M1Q7fDPPGfe15L9zS"));
    }
}
