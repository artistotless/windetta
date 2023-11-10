using Windetta.Common.Testing;
using Windetta.Common.Types;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxnsTests.Mocks;

public class TonServiceMock : MockInitializator<IWithdrawService>
{
    protected override void Setup(Mock<IWithdrawService> mock)
    {
        mock.Setup(x => x.ExecuteWithdraw(It.IsAny<WalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));
    }
}