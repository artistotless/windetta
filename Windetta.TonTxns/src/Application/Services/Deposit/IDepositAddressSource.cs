using Windetta.Common.Types;

namespace Windetta.TonTxns.Application.Services;

public interface IDepositAddressSource : ISingletonService
{
    ValueTask<TonAddress> GetAddressAsync();
}
