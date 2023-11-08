using Windetta.Common.Testing;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxnsTests.Mocks;

public class TxnsServiceMock : MockInitializator<ITransactionsService>
{
    public static Guid ExistingTxn = Guid.NewGuid();
    public static Guid NonExistingTxn = Guid.NewGuid();

    protected override void Setup(Mock<ITransactionsService> mock)
    {
        mock.Setup(x => x.ExistAsync(It.Is<Guid>(x => x == ExistingTxn)))
            .Throws(new Exception(nameof(ExistingTxn)));

        mock.Setup(x => x.ExistAsync(It.Is<Guid>(x => x == NonExistingTxn)))
            .ReturnsAsync(false);
    }
}