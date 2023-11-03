using Windetta.Common.Types;
using Windetta.TonTxns.Infrastructure.Models;

namespace Windetta.TonTxns.Infrastructure.Services;

public interface IWalletCredentialSource : ISingletonService
{
    public TonWalletCredential Value { get; set; }
}
