using MassTransit;
using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface ITransferTon : CorrelatedBy<Guid>
{
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
}
