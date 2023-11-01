using MassTransit;
using Windetta.Contracts.Commands;
using Windetta.TonTxns.Infrastructure.Models;
using Windetta.TonTxns.Infrastructure.Services;

namespace Windetta.TonTxns.Consumers
{
    public class TransferConsumer : IConsumer<Batch<ITransferTon>>
    {
        private readonly ITonService _tonService;
        private readonly IWalletCredentialSource _walletCredentialSource;

        public TransferConsumer(ITonService tonService, IWalletCredentialSource walletCredentialSource)
        {
            _tonService = tonService;
            _walletCredentialSource = walletCredentialSource;
        }

        public async Task Consume(ConsumeContext<Batch<ITransferTon>> context)
        {
            var credential = await _walletCredentialSource.GetCredential();
            var transferMessages = LoadTransferMessageArray(context.Message);

            await _tonService.TransferTon(credential, transferMessages);
        }

        private IEnumerable<TransferMessage> LoadTransferMessageArray(Batch<ITransferTon> batch)
        {
            foreach (var item in batch)
                yield return new TransferMessage(item.Message.Destination, item.Message.Nanotons);
        }
    }
}