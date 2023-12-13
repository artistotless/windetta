using TonSdk.Client;
using TonSdk.Contracts.Wallet;
using TonSdk.Core;
using TonSdk.Core.Block;
using TonSdk.Core.Crypto;
using Windetta.Common.Ton;
using Windetta.TonTxnsTests.Fixtures;

namespace Windetta.TonTxnsTests.TonApi;

[Collection("TonTestsCollection")]
public class SmartContractTests
{
    private readonly TonClient _client;
    private const uint sub_wallet_id = 698983191;
    private const string _dest = "EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV";

    public SmartContractTests(TonClientFixture tonClientFixture)
    {
        _client = tonClientFixture.Client;
    }

    [Fact]
    public async Task Should_NotCrash_ToSend_Massive_Txn_HighloadV1Custom()
    {
        var mn = new Mnemonic(new[] { "better", "air", "abstract", "bacon", "bird", "auto", "also", "awkward", "business", "barrel", "between", "boring", "avocado", "assault", "baby", "arch", "baby", "already", "awkward", "boy", "answer", "bonus", "adapt", "ask", });
        (string publicKey, string privateKey) = mn.GetKeysPair();

        // address - EQCY-EUrQiEgy8a2JI5PNfv3itWseEh1TtzrPsIRDpH8iS2H
        var wallet = new HighloadV1Custom(new()
        {
            PublicKey = Convert.FromBase64String(publicKey),
            SubwalletId = sub_wallet_id,
            Workchain = 0
        });


        var seqno = await _client.Wallet.GetSeqno(wallet.Address) ?? 0;

        ExternalInMessage transferMessage;

        if (seqno == 0)
            transferMessage = wallet.CreateDeployMessage();
        else
        {
            transferMessage = wallet.CreateTransferMessage(new[]
           {
            CreateIntTransferMessage(_dest, new Coins(0.0001),1),
            }, seqno);
        }

        transferMessage = transferMessage.Sign(Convert.FromBase64String(privateKey));

        (await _client.SendBoc(transferMessage.Cell!)).Type.ToLower().ShouldBe("ok");
    }

    [Fact]
    public async Task WalletData_ShouldBe_Null_IfNotDeployed()
    {
        var walletInfo = (await _client.GetAddressInformation(new("EQDdUqwx8t5u6-lj_bfcGnziTHHP_K53b8k4xpPQqY1Dmt-2")));

        walletInfo.Data.ShouldBeNull();
    }

    [Fact]
    public async Task WalletState_ShouldBe_Uninit_IfNotDeployed()
    {
        var walletInfo = (await _client.GetAddressInformation(new("EQDdUqwx8t5u6-lj_bfcGnziTHHP_K53b8k4xpPQqY1Dmt-2")));

        walletInfo.State.ShouldBe(AccountState.Uninit);
    }

    private WalletTransfer CreateIntTransferMessage(string destination, Coins coins, byte mode)
    {
        return new WalletTransfer
        {
            Message = new InternalMessage(new()
            {
                Info = new IntMsgInfo(new()
                {
                    Dest = new Address(destination),
                    Value = coins,
                    Bounce = false
                }),
                //Body = new CellBuilder()
                //.StoreUInt(0, 32)
                //.StoreString("c96a90c9-7f24-4e8d-83e6-18bb5488fe86").Build()
            }),
            Mode = mode
        };
    }
}
