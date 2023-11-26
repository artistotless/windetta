using Windetta.Common.Types;
using Windetta.TonTxns.Application.Models;

namespace Windetta.TonTxns.Application.Services;

public interface IWalletCredentialSource : ISingletonService
{
    public WalletCredential Value { get; set; }
}
