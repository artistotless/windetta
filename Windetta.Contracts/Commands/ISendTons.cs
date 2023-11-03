using MassTransit;
using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface ISendTons : CorrelatedBy<Guid>
{
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
}

public interface IPackedSendTons : CorrelatedBy<Guid>
{
    public ISendTons[] Sends { get; set; }
}
