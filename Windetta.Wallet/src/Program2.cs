using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TonLibDotNet;
using TonLibDotNet.Types;
using TonLibDotNet.Types.Msg;
using TonLibDotNet.Types.Wallet;
using Windetta.Common.RabbitMQ;
using Windetta.Common.Types;
using Windetta.Wallet.Messages.Events;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
{
    builder.AddRabbitMq();
    builder.ResolveDependenciesFromAssembly();
}));

var app = builder.Build();

app.MapGet("/", () => "Windetta.Wallet Service");

////==================================

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

//string destination = "EQAlIkyraDxe2Hucg6c2rFnYrwCtfl_WLf88yjxnGzWeE56U"; // TON Foundation wallet
//// Obtain client from DI

//using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
//    .SetMinimumLevel(LogLevel.Trace)
//    .AddConsole());

//ILogger<TonClient> logger = loggerFactory.CreateLogger<TonClient>();

//IOptions<TonOptions> optionParameter = Microsoft.Extensions.Options.Options.Create(new TonOptions()
//{
//    UseMainnet = true,
//    LogTextLimit = 500, // Set to 0 to see full requests/responses
//    VerbosityLevel = 0,
//    Options = new TonLibDotNet.Types.Options()
//    {
//        KeystoreType = new KeyStoreTypeDirectory("D:/Temp/keys")
//    }
//});

//var tonClient = new TonClient(logger, optionParameter);

//// You need to init it before first use.
//// During this, TON network config file is loaded from internet.
//// Subsequent calls to `InitIfNeeded` will be ignored, 
////   so no need for you to have additional variable 
////   to remember that you already called it.


//await tonClient.InitIfNeeded();

//// Use 'Execute' to send requests.

//var walletId = tonClient.OptionsInfo.ConfigInfo.DefaultWalletId;
//walletId = 698983191;

//// some "random" bytes
//var localPass = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });
//var mnemonicPass = Convert.ToBase64String(new byte[] { 19, 42, 148 });
//var randomExtra = Convert.ToBase64String(new byte[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });
//var keyPass = Convert.ToBase64String(new byte[] { 21, 3, 7, 11 });

//// create new key
//var key = await tonClient.CreateNewKey();
//logger.LogInformation("New key: public = {PublicKey}, secret = {Secret}", key.PublicKey, key.Secret);

//var ek = await tonClient.ExportKey(key);
//logger.LogInformation("Mnemonic for this key is: {Words}", string.Join(" ", ek.WordList));

////var epk = await tonClient.ExportPemKey(key, localPass, keyPass);
////logger.LogInformation("Same key in PEM with password:\r\n{PEM}", epk.Pem);

////var eek = await tonClient.ExportEncryptedKey(key, localPass, keyPass);
////logger.LogInformation("Same key exported with password: {Value}", eek.Data);

////var euk = await tonClient.ExportUnencryptedKey(key, localPass);
////logger.LogInformation("Same key in unencrypted form: {Value}", euk.Data);


//var inputKey = new Key()
//{
//    PublicKey = "Pua7nqPK6q21tTSntzzJJEQiZWCrTH2G-42T2nsMubobqSYC",
//    Secret = "8NzoSPV3HZS/FgvBv72hPp3KtWPoTeWpzvaMZKV1kVo="
//};

//var initialAccountState = new V3InitialAccountState() { PublicKey = inputKey.PublicKey, WalletId = walletId };
//var address = await tonClient.GetAccountAddress(initialAccountState, 0, 0);

//// Step 2: Build message and action
//var msg = new Message(new AccountAddress(destination))
//{
//    Data = new DataText(Data.Int256_AllZeroes),
//    Amount = TonUtils.Coins.ToNano(0.799444445M),
//    SendMode = 1,
//};

//var action = new ActionMsg(new[] { msg }.ToList()) { AllowSendToUninited = true };

//var ast = await tonClient.GetAccountState(address.Value);
//logger.LogInformation("Acc info via GetAccountState(): balance = {Value} nanoton or {Value} TON", ast.Balance, TonUtils.Coins.FromNano(ast.Balance));

//// Step 3: create query and send it
//var query = await tonClient.CreateQuery(new InputKeyRegular(inputKey), address, action, TimeSpan.FromMinutes(1), initialAccountState: initialAccountState);

//// wanna know fees before sending?
//var fees = await tonClient.QueryEstimateFees(query.Id);
//logger.LogInformation("Estimated fees (in nanoton): InFwdFee={Value}, FwdFee={Value}, GasFee={Value}, StorageFee={Value}", fees.SourceFees.InFwdFee, fees.SourceFees.FwdFee, fees.SourceFees.GasFee, fees.SourceFees.StorageFee);
//var fee = TonUtils.Coins.FromNano(fees.SourceFees.FwdFee + fees.SourceFees.InFwdFee + fees.SourceFees.GasFee + fees.SourceFees.StorageFee + fees.DestinationFees.Sum(x => x.InFwdFee + x.FwdFee + x.GasFee + x.StorageFee));
//// Send it to network. You dont have TX id or something in response - just poll getTransactions() for your account and wait for new TX.
//;
//_ = await tonClient.QuerySend(query.Id);
//logger.LogInformation("Send OK. Check your account in explorer");
////==================================


app.Run();
