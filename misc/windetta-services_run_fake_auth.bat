E:
set FAKE_AUTH=Enabled
set ASPNETCORE_ENVIRONMENT=Development
set IS_MINIMIZED=1
set ASK_DEBUGGING_ATTACH=No

cd E:\Projects\windetta\Windetta.Main\src\bin\Debug\net8.0\
start /min "Main:55001" dotnet Windetta.Main.dll

cd E:\Projects\windetta\lspm\lspm\src\bin\Debug\net8.0\
start /min "LSPM:55005" dotnet LSPM.dll

cd E:\Projects\windetta\Windetta.Wallet\src\bin\Debug\net8.0\
start /min "Wallet:55003" dotnet Windetta.Wallet.dll

cd E:\Projects\windetta\Windetta.Identity\src\bin\Debug\net8.0\
start /min "Identity:55002" dotnet Identity.dll

cd E:\Projects\windetta\Windetta.Web\src\bin\Debug\net9.0\
start /min "Web:55004" dotnet Windetta.Web.dll

exit