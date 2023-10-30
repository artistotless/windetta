using AutoFixture;
using Moq;
using Windetta.Common.Testing;
using Windetta.Common.Types;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Application.Services;

namespace Windetta.WalletTests.Mocks;

internal class UserWalletServiceMock : MockInitializator<IUserWalletService>
{
    protected override void Setup(Mock<IUserWalletService> mock)
    {
        mock.Setup(x => x.CreateWalletAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Fixture().Create<WalletInfoDto>());

        mock.Setup(x => x.TransferAsync(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<Guid>()));
        mock.Setup(x => x.WithdrawAsync(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<TonAddress>()));
        mock.Setup(x => x.HoldBalanceAsync(It.IsAny<Guid>(), It.IsAny<long>()));
        mock.Setup(x => x.UnHoldBalanceAsync(It.IsAny<Guid>()));
    }
}
