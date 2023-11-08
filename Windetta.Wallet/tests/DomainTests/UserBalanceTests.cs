using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Wallet.Domain;

namespace Windetta.WalletTests.DomainTests;

public class UserBalanceTests
{
    [Fact]
    public void ShouldIncreaseAmount()
    {
        // arrange
        var entity = new UserBalance();

        // act
        entity.Increase(100);

        // assert
        entity.Amount.ShouldBe(100);
    }

    [Fact]
    public void ShouldDecreaseAmount()
    {
        // arrange
        var entity = new UserBalance();

        // act
        entity.Increase(400);
        entity.Decrease(100);

        // assert
        entity.Amount.ShouldBe(300);
    }

    [Fact]
    public void DecreaseShouldThrowExceptionIfFundsNotEnough()
    {
        // arrange
        var entity = new UserBalance();

        // act
        entity.Increase(100);

        // assert
        Should.Throw<WindettaException>(() => entity.Decrease(101))
            .ErrorCode.ShouldBe(Errors.Wallet.FundsNotEnough);
    }

    [Fact]
    public void ShouldThrowsExceptionIfHoldTwiceAndMore()
    {
        // arrange
        var entity = new UserBalance();

        // act
        entity.Increase(100);
        entity.Hold(20);

        // assert
        Should.Throw<WindettaException>(() => entity.Hold(30))
            .ErrorCode.ShouldBe(Errors.Wallet.FundsAlreadyHeld);
    }
}
