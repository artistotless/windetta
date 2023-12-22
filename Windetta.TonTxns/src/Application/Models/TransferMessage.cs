using Windetta.Contracts;

namespace Windetta.TonTxns.Application.Models;

public record TransferMessage(TonAddress destination, ulong nanotons);
