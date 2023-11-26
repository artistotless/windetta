using Windetta.Common.Types;

namespace Windetta.TonTxns.Application.Models;

public record TransferMessage(TonAddress destination, ulong nanotons);
