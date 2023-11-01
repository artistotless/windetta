using Windetta.Common.Testing;
using Windetta.Common.Types;
using Windetta.TonTxns.Infrastructure.Models;
using Windetta.TonTxns.Infrastructure.Services;

namespace Windetta.TonTxnsTests.Mocks;

internal class TonServiceMock : MockInitializator<ITonService>
{
    protected override void Setup(Mock<ITonService> mock)
    {
        mock.Setup(x => x.GetBalance(It.IsAny<TonAddress>()))
            .ReturnsAsync(Random.Shared.Next(0, 100000));

        mock.Setup(x => x.TransferTon(It.IsAny<TonWalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));
    }
}
