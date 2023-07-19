using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TonLibDotNet;
using TonLibDotNet.Types;
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

//==================================

// Generate a new mnemonic phrase
var mnemonics = new[]{
"breeze",
"find",
"cliff",
"nuclear",
"vendor",
"timber",
"easily",
"control",
"elbow",
"husband",
"consider",
"various",
"boat",
"behave",
"stamp",
"river",
"custom",
"swamp",
"frequent",
"often",
"benefit",
"recycle",
"sample",
"surround"};

// Obtain client from DI

using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
    .AddConsole());

ILogger<TonClient> logger = loggerFactory.CreateLogger<TonClient>();

IOptions<TonOptions> optionParameter = Microsoft.Extensions.Options.Options.Create(new TonOptions()
{
    UseMainnet = true,
    LogTextLimit = 500, // Set to 0 to see full requests/responses
    VerbosityLevel = 0,
    Options = new TonLibDotNet.Types.Options()
    {
        KeystoreType = new KeyStoreTypeDirectory("D:/Temp/keys")
    }
});

var tonClient = new TonClient(logger, optionParameter);

// You need to init it before first use.
// During this, TON network config file is loaded from internet.
// Subsequent calls to `InitIfNeeded` will be ignored, 
//   so no need for you to have additional variable 
//   to remember that you already called it.
await tonClient.InitIfNeeded();

// Use 'Execute' to send requests.

var lsi = await tonClient.LiteServerGetInfo();
;
logger.LogInformation("Server time: {Now}", lsi.Now);

//==================================

app.UseRabbitMq()
    .SubscribeEvent<UserCreated>();

app.Run();
