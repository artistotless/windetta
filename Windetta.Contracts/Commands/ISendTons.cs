using MassTransit;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface ISendTons : CorrelatedBy<Guid>, ICommand
{
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
}

public interface IPackedSendTons : CorrelatedBy<Guid>, ICommand
{
    public ISendTons[] Sends { get; set; }
}
