using TonSdk.Contracts.Wallet;
using TonSdk.Core.Crypto;
using Windetta.Common.Ton;
using Windetta.TonTxnsTests.Fixtures;

namespace Windetta.TonTxnsTests.TonApi;

public class MnemonicTests
{
    [Fact]
    public void ShouldGenerateValidWalletV3Data()
    {
        // arrange
        var mnemonic = new Mnemonic(WalletV3ValidDataExample.Words);
        // act
        var wallet = new WalletV3(new WalletV3Options()
        {
            PublicKey = mnemonic.Keys.PublicKey,
            Workchain = 0,
            SubwalletId = 698983191
        });

        (string publicKey, string privateKey) = mnemonic.GetKeysPair();
        var address = wallet.Address.ToString();

        // assert
        publicKey.ShouldBe(WalletV3ValidDataExample.ValidPublicKey);
        privateKey.ShouldBe(WalletV3ValidDataExample.ValidPrivateKey);
        address.ToString().ShouldBe(WalletV3ValidDataExample.ValidAddress);
    }
}
