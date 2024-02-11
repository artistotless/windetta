using Windetta.Common.Testing;
using Windetta.Contracts;
using Windetta.Contracts.Responses;
using Windetta.Main.Core.Services.Wallet;

namespace Windetta.MainTests.Mocks;

public class WalletServiceMock : MockInitializator<IWalletService>
{
    public bool ReturnIsEqualOrGreater { get; init; } = true;
    public UserBalanceResponse ReturnBalance { get; init; } = new(amount: 1000, heldAmount: 0);

    protected override void Setup(Mock<IWalletService> mock)
    {
        mock
            .Setup(x => x.IsEqualOrGreater(It.IsAny<Guid>(), It.IsAny<FundsInfo>()))
            .ReturnsAsync(ReturnIsEqualOrGreater);
        mock
            .Setup(x => x.GetBalance(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(ReturnBalance);
    }
}