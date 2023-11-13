using Windetta.Common.Testing;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxnsTests.Mocks;

public class TonServiceMock : MockInitializator<IWithdrawService>
{
    private bool _useFailCase = false;

    public TonServiceMock(bool useFailCase = false)
    {
        _useFailCase = useFailCase;
    }

    protected override void Setup(Mock<IWithdrawService> mock)
    {
        mock.Setup(x => x.ExecuteWithdraw(It.IsAny<WalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));

        mock.Setup(x => x.ExecuteWithdraw(It.Is<WalletCredential>(x => _useFailCase), It.IsAny<IEnumerable<TransferMessage>>()))
            .ThrowsAsync(new Exception(nameof(_useFailCase)));

    }
}