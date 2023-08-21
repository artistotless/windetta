//using Autofac.Extensions.DependencyInjection;
//using System.Diagnostics;
//using System.Text;
//using TonSdk.Client;
//using TonSdk.Contracts.Wallet;
//using TonSdk.Core;
//using TonSdk.Core.Block;
//using TonSdk.Core.Boc;
//using TonSdk.Core.Crypto;
//using Windetta.Common.RabbitMQ;
//using Windetta.Common.Types;
//using Windetta.Wallet.Messages.Events;

//var builder = WebApplication.CreateBuilder(args);
//var services = builder.Services;

//builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
//{
//    builder.AddRabbitMq();
//    builder.ResolveDependenciesFromAssembly();
//}));

//var app = builder.Build();

//app.MapGet("/", () => "Windetta.Wallet Service");

////==================================

//// Create a new instance of the TonClient using the specified endpoint and API key
//TonClient tonclient = new TonClient(new TonClientParameters { Endpoint = "https://toncenter.com/api/v2/jsonRPC", ApiKey = "64371a7e9bd3c6e5245e19313b8ccc9f904ed22f8628b1fd0ce8cad817b69183" });
//// Generate a new mnemonic phrase

//var mnemonics = new[]{
//"breeze",
//"find",
//"cliff",
//"nuclear",
//"vendor",
//"timber",
//"easily",
//"control",
//"elbow",
//"husband",
//"consider",
//"various",
//"boat",
//"behave",
//"stamp",
//"river",
//"custom",
//"swamp",
//"frequent",
//"often",
//"benefit",
//"recycle",
//"sample",
//"surround"};

//Mnemonic mnemonic = new Mnemonic(mnemonics);

//var seed = Mnemonic.GenerateSeed(mnemonics);
//var seedStr = Encoding.UTF8.GetString(seed);
//var publicKey = Utils.BytesToHex(mnemonic.Keys.PublicKey);
//var privateKey = Utils.BytesToHex(mnemonic.Keys.PrivateKey);

//var tons = TonLibDotNet.TonUtils.Coins.FromNano(1888000000);
//// Create a new preprocessed wallet using the public key from the generated mnemonic
//PreprocessedV2 wallet = new PreprocessedV2(new PreprocessedV2Options { PublicKey = mnemonic.Keys.PublicKey! });

//// Get the address associated with the wallet
//Address address = wallet.Address;

//foreach (var key in mnemonic.Words)
//    Debug.WriteLine(key);

//Debug.WriteLine("-----------------");
//Debug.WriteLine(address);

//// Convert the address to a non-bounceable format
//string nonBounceableAddress = address.ToString(AddressType.Base64, new AddressStringifyOptions(false, false, true));

//// Retrieve the wallet data
//Cell? walletData = (await tonclient.GetAddressInformation(address)).Data;

//// Extract the sequence number from the wallet data, or set it to 0 if the data is null
//uint seqno = walletData == null ? 0 : wallet.ParseStorage(walletData.Parse()).Seqno;

//// Get the balance of the wallet
//Coins walletBalance = await tonclient.GetBalance(address);

//// Get the destination address for the transfer from the Ton DNS
//Address destination = new Address("EQCNkSLURL98zKoKQeEoMSCb7uMO5JFWF5CEaJ-f1baspjA2")
//;
//// Create a transfer message for the wallet
//ExternalInMessage message = wallet.CreateTransferMessage(new[]
//{
//    new WalletTransfer
//    {
//        Message = new InternalMessage(new()
//        {
//            Info = new IntMsgInfo(new()
//            {
//                Dest = destination,
//                Value = new Coins("0,794839103")
//            }),
//            Body = new CellBuilder().StoreUInt(0, 32).Build()
//        }),
//        Mode = 1
//    }
//}, seqno).Sign(mnemonic.Keys.PrivateKey, true);

//// Send the serialized message
//var res = await tonclient.SendBoc(message.Cell!);
//;
////==================================

//app.UseRabbitMq()
//    .SubscribeEvent<UserCreated>();

//app.Run();
