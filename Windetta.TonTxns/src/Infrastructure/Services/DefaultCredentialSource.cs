using Windetta.Common.Configuration;
using Windetta.TonTxns.Infrastructure.Models;

namespace Windetta.TonTxns.Infrastructure.Services;

public class DefaultCredentialSource : IWalletCredentialSource
{
    public TonWalletCredential Value { get; set; }

    public DefaultCredentialSource(IConfiguration configuration)
    {
        SetCredential(configuration);
    }

    private void SetCredential(IConfiguration configuration)
    {
        Value = configuration.GetOptions<TonWalletCredential>(nameof(TonWalletCredential));
    }
}