using Shouldly;
using TonSdk.Client;
using TonSdk.Contracts.Wallet;
using TonSdk.Core;
using TonSdk.Core.Block;
using TonSdk.Core.Boc;
using TonSdk.Core.Crypto;
using Windetta.WalletTests.Fixtures;

namespace Windetta.WalletTests.TonApi;

[Collection("TonTestsCollection")]
public class SmartContractTests
{
    private readonly TonClient _client;

    // Since this account has not been deployed on chain
    // it has 0 balance and Uninit state
    private const string address = "EQDrouFpSZXj0X7bbUwBDb8K0ciY2hiR3X07935zDF6R-40r";

    public SmartContractTests(TonClientFixture tonClientFixture)
    {
        _client = tonClientFixture.Client;
    }

    [Fact]
    public async Task DeployTest()
    {
        var publicKey = "rCLUIUjPtDSOFE16RgcYCujiWEpC0qfQZYkaoFBZXyE=";
        var address = "EQCRD7t48BId3EIA3YMJqi1xrXH2Ra5ehkgHKLx4mZ1B_pk5";
        var privateKey = "+99hGH8o1ZhDKfFBdxJ0lbT3N8m8tQZy5rJskZBzSB0=";

        string[] words = { "barely", "allow", "boring", "achieve", "boss", "boss", "agree", "begin", "ancient", "base", "betray", "business", "butter", "attract", "address", "barely", "allow", "author", "bubble", "bring", "arrange", "around", "brisk", "awful" };

        var seqno = await _client.Wallet.GetSeqno(new(address)) ?? 0;

        var bodyCell = new CellBuilder()
            .StoreUInt(698983191, 32) // sub_wallet_id
            .StoreInt(-1L, 32) // valid_until
            .StoreUInt(seqno, 32) // seqno
            .Build();

        var stateInitDataCell = new CellBuilder()
            .StoreUInt(0uL, 32) // init seqno
            .StoreUInt(698983191, 32) // sub_wallet_id
            .StoreBytes(Convert.FromBase64String(publicKey)) // public_key
            .Build();

        StateInitOptions stateInitOpt = new StateInitOptions()
        {
            // code of smart contract (WalletV3R2)
            Code = Cell.From("B5EE9C724101010100710000DEFF0020DD2082014C97BA218201339CBAB19F71B0ED44D0D31FD31F31D70BFFE304E0A4F2608308D71820D31FD31FD31FF82313BBF263ED44D0D31FD31FD3FFD15132BAF2A15144BAF2A204F901541055F910F2A3F8009320D74A96D307D402FB00E8D101A4C8CB1FCB1FCBFFC9ED5410BD6DAD"),
            // data that will be stored in smart contract storage
            Data = stateInitDataCell,
            SplitDepth = null,
            Special = null,
            Library = null,
        };

        ExtInMsgInfoOptions infoOpt = new ExtInMsgInfoOptions()
        {
            Dest = new Address(address),
            Src = null,
            ImportFee = new Coins(0),
        };

        ExternalInMessageOptions opt = new ExternalInMessageOptions()
        {
            Info = new ExtInMsgInfo(infoOpt),
            StateInit = new StateInit(stateInitOpt),
            Body = bodyCell,
        };

        ExternalInMessage msg = new ExternalInMessage(opt);

        msg.Sign(Convert.FromBase64String(privateKey), false);

        var result = await _client.SendBoc(msg.Cell);
    }

    [Fact]
    public async Task ShouldTransferCoinsWithoudDeploying()
    {
        WalletV3 wallet = new WalletV3(new WalletV3Options
        {
            PublicKey = Convert.FromBase64String(WalletV3ValidDataExample.ValidPublicKey)
        });

        // Retrieve the wallet data
        var walletData = (await _client.GetAddressInformation(wallet.Address)).Data;

        // Extract the sequence number from the wallet data, or set it to 0 if the data is null
        var seqno = walletData == null ? 0 : wallet.ParseStorage(walletData.Parse()).Seqno;

        // Create a transfer message for the wallet
        ExternalInMessage message = wallet.CreateTransferMessage(new[]{
            CreateTransferMessage(address,new Coins(0.01)),
            CreateTransferMessage(address,new Coins(0.01)),
            CreateTransferMessage(address,new Coins(0.01)),
        }
        , seqno)
           .Sign(Convert.FromBase64String(WalletV3ValidDataExample.ValidPrivateKey));

        // Send the serialized message
        (await _client.SendBoc(message.Cell!)).Type.ToLower().ShouldBe("ok");
    }

    [Fact]
    public async Task WalletData_ShouldBe_Null_IfNotDeployed()
    {
        var walletInfo = (await _client.GetAddressInformation(new(address)));

        walletInfo.Data.ShouldBeNull();
    }

    [Fact]
    public async Task WalletState_ShouldBe_Uninit_IfNotDeployed()
    {
        var walletInfo = (await _client.GetAddressInformation(new(address)));

        walletInfo.State.ShouldBe(AccountState.Uninit);
    }

    private WalletTransfer CreateTransferMessage(string destination, Coins coins)
    {
        return new WalletTransfer
        {
            Message = new InternalMessage(new()
            {
                Info = new IntMsgInfo(new()
                {
                    Dest = new Address(destination),
                    Value = coins
                }),
                Body = new CellBuilder()
                .StoreUInt(0, 32)
                .StoreString(string.Empty)
                .Build(),
            }),
            Mode = 1
        };
    }
}
