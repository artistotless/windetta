using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.TonTxns.Application.Consumers;

public class SendTonsException : WindettaException
{
    public SendTonsException(string message) : base(Errors.TonTxns.TonTransferError)
    {

    }
}
