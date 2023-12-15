using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Wallet.Domain;

namespace Windetta.WalletTests.DomainTests;

public class UserWalletTests
{
    [Fact]
    public void GetBalance_ShouldThrowExceptionIfBalanceNotFound()
    {
        // arrange
        var wallet = new UserWallet();

        // act
        var exception = Should.Throw<WindettaException>(
            () => wallet.GetBalance(currencyId: 1));

        // assert
        exception.ErrorCode.ShouldBe(Errors.Wallet.BalanceNotFound);
    }

    [Fact]
    public void GetBalance_ShouldReturnBalance()
    {
        // arrange
        var currencyId = 1;
        var wallet = new UserWallet();

        wallet.Balances = new List<UserBalance>
        {
            new UserBalance()
            {
                CurrencyId = currencyId,
            }
        };

        // act
        var balance = wallet.GetBalance(currencyId);

        // assert
        balance.ShouldNotBeNull();
        balance.CurrencyId.ShouldBe(currencyId);
    }

    [Fact]
    public void Transfer_ShouldThrowExceptionIfBalanceNotFound()
    {
        // arrange
        var currencyId = 1;
        var walletSource = new UserWallet();
        var walletDestination = new UserWallet();

        walletSource.Balances = new List<UserBalance>
        {
            new UserBalance(){CurrencyId = currencyId}
        };

        // act
        var exception = Should.Throw<WindettaException>(
            () => walletSource.TransferToWallet(walletDestination, new FundsInfo(currencyId,100)));

        // assert
        exception.ShouldNotBeNull();
        exception.ErrorCode.ShouldBe(Errors.Wallet.BalanceNotFound);
    }

    [Fact]
    public void Transfer_ShouldThrowExceptionIfAmountNotEnough()
    {
        // arrange
        var currencyId = 1;

        var walletSource = new UserWallet();
        var walletDestination = new UserWallet();

        var walletSourceBalance = new UserBalance()
        {
            CurrencyId = currencyId,
        };

        var walletDestinationBalance = new UserBalance()
        {
            CurrencyId = currencyId,
        };

        walletSourceBalance.Increase(10);

        walletSource.Balances = new List<UserBalance>
        {
            walletSourceBalance
        };

        walletDestination.Balances = new List<UserBalance>
        {
            walletSourceBalance
        };

        // act
        var exception = Should.Throw<WindettaException>(
            () => walletSource.TransferToWallet(walletDestination, new FundsInfo(currencyId, 100)));

        // assert
        exception.ShouldNotBeNull();
        exception.ErrorCode.ShouldBe(Errors.Wallet.FundsNotEnough);
    }
}
