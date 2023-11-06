using Windetta.Common.Types;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxns.Infrastructure.Services;

public class DefaultDepositAddressSource : IDepositAddressSource
{
    private TonAddress _value { get; set; }

    public DefaultDepositAddressSource(IConfiguration configuration)
    {
        SetAddress(configuration);
    }

    private void SetAddress(IConfiguration configuration)
    {
        _value = new(configuration.GetValue<string>("TonDepositAddress")??
            "EQCIWER_3F987_97Ox_Qa0GObs2sIQHYjUuJ5GpqGbvYji0U");
    }

    public ValueTask<TonAddress> GetAddressAsync()
        => ValueTask.FromResult(_value);
}
