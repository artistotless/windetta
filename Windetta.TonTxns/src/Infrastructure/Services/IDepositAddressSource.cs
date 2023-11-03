using Windetta.Common.Types;

namespace Windetta.TonTxns.Infrastructure.Services;

public interface IDepositAddressSource : ISingletonService
{
    ValueTask<TonAddress> GetAddressAsync();
}
