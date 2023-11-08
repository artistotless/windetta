using Windetta.Common.Testing;
using Windetta.Common.Types;
using Windetta.TonTxns.Application.Models;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxnsTests.Mocks;

public class TonServiceMock : MockInitializator<ITonService>
{
    protected override void Setup(Mock<ITonService> mock)
    {
        mock.Setup(x => x.GetBalance(It.IsAny<TonAddress>()))
            .ReturnsAsync(Random.Shared.Next(0, 100000));

        mock.Setup(x => x.SendTons(It.IsAny<TonWalletCredential>(), It.IsAny<IEnumerable<TransferMessage>>()));
    }
}