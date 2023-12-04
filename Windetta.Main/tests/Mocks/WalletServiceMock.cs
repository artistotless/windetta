using Windetta.Common.Testing;
using Windetta.Main.Services;

namespace Windetta.MainTests.Mocks;

public class WalletServiceMock : MockInitializator<IWalletService>
{
    protected override void Setup(Mock<IWalletService> mock)
    {
        mock
            .Setup(x => x.HoldBalance(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<ulong>()))
            .Returns(Task.CompletedTask);
        mock
            .Setup(x => x.UnHoldBalance(It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        mock
            .Setup(x => x.IsEqualOrGreater(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<ulong>()))
            .ReturnsAsync(true);
        mock
            .Setup(x => x.GetBalance(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(new UserBalance()
            {
                Amount = 1000,
                HeldAmount = 0
            });
    }
}