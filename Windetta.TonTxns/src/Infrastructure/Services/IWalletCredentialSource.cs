using Windetta.Common.Configuration;
using Windetta.Common.Types;
using Windetta.TonTxns.Infrastructure.Models;

namespace Windetta.TonTxns.Infrastructure.Services;

public interface IWalletCredentialSource : ISingletonService
{
    ValueTask<TonWalletCredential> GetCredential();
}

public class DefaultCredentialSource : IWalletCredentialSource
{
    private readonly IConfiguration _configuration;

    public DefaultCredentialSource(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ValueTask<TonWalletCredential> GetCredential()
    {
        return ValueTask.FromResult(
            _configuration.GetOptions<TonWalletCredential>(nameof(TonWalletCredential)));
    }
}