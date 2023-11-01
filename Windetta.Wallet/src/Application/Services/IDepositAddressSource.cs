using Windetta.Common.Types;

namespace Windetta.Wallet.Application.Services;

public interface IDepositAddressSource : IScopedService
{
    ValueTask<TonAddress> GetAddressAsync();
}
