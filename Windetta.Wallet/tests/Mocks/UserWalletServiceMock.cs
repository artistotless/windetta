using Windetta.Common.Testing;
using Windetta.Contracts;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Domain;

namespace Windetta.WalletTests.Mocks;

internal class UserWalletServiceMock : MockInitializator<IUserWalletService>
{
    protected override void Setup(Mock<IUserWalletService> mock)
    {
        mock.Setup(x => x.CreateWalletAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<UserBalance>>()));
        mock.Setup(x => x.TransferAsync(It.IsAny<TransferArgument>()));
        mock.Setup(x => x.DeductAsync(It.IsAny<DeductArgument>()));
        mock.Setup(x => x.TopUpBalance(It.IsAny<TopUpArgument>()));
        mock.Setup(x => x.HoldBalanceAsync(It.IsAny<Guid>(), It.IsAny<FundsInfo>()));
        mock.Setup(x => x.UnHoldBalanceAsync(It.IsAny<Guid>(), It.IsAny<int>()));
    }
}
