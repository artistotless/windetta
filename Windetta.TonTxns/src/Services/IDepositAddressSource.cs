using Windetta.Common.Types;

namespace Windetta.TonTxns.Application.Services;

public interface IDepositAddressSource : IScopedService
{
    ValueTask<TonAddress> GetAddressAsync();
}
