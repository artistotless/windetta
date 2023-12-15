using Windetta.Common.Testing;
using Windetta.TonTxns.Application.Services.Audit;

namespace Windetta.TonTxnsTests.Mocks;

public class TxnsServiceMock : MockInitializator<IWithdrawals>
{
    public bool UseExistingTxnCase = false;

    public TxnsServiceMock(bool useExistingTxnCase = false)
    {
        UseExistingTxnCase = useExistingTxnCase;
    }

    protected override void Setup(Mock<IWithdrawals> mock)
    {
        mock.Setup(x => x.ExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        mock.Setup(x => x.ExistAsync(It.Is<Guid>(x => UseExistingTxnCase)))
            .ThrowsAsync(new Exception(nameof(UseExistingTxnCase)));
    }
}