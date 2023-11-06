using Windetta.Common.Testing;
using Windetta.TonTxns.Application.Services;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxnsTests.Mocks;

internal class TxnsServiceMock : MockInitializator<ITransactionsService>
{
    public Guid ExistingTxn = Guid.NewGuid();
    public Guid NonExistingTxn = Guid.Empty;

    protected override void Setup(Mock<ITransactionsService> mock)
    {
        mock.Setup(x => x.ExistAsync(It.Is<Guid>(x => x == ExistingTxn)))
            .ReturnsAsync(true);

        mock.Setup(x => x.ExistAsync(It.Is<Guid>(x => x == NonExistingTxn)))
            .ReturnsAsync(true);

        mock.Setup(x => x.AddAsync(It.IsAny<Transaction>()));
        mock.Setup(x => x.UpdateAsync(It.IsAny<Transaction>()));
    }
}