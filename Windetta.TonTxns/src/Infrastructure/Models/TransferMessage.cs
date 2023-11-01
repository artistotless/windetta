using Windetta.Common.Types;

namespace Windetta.TonTxns.Infrastructure.Models;

public record TransferMessage(TonAddress destination, long nanotons);
