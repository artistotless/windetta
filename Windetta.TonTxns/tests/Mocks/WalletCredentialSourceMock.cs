using Windetta.Common.Testing;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxnsTests.Mocks;

public class WalletCredentialSourceMock : MockInitializator<IWalletCredentialSource>
{
    protected override void Setup(Mock<IWalletCredentialSource> mock)
    {
        mock.Setup(x => x.Value)
            .Returns(new Fixture().Create<WalletCredential>());
    }
}