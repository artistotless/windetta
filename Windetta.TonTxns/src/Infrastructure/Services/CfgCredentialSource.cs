using Windetta.Common.Configuration;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxns.Infrastructure.Services;

public class CfgCredentialSource : IWalletCredentialSource
{
    public WalletCredential Value { get; set; }

    public CfgCredentialSource(IConfiguration configuration)
    {
        SetCredential(configuration);
    }

    private void SetCredential(IConfiguration configuration)
    {
        Value = configuration.GetOptions<WalletCredential>(nameof(WalletCredential));
    }
}