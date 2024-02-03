using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface ISendTons : CorrelatedBy<Guid>, ICommand
{
    public ulong Nanotons { get; set; }
    public TonAddress Destination { get; set; }
}

public interface IPackedSendTons : CorrelatedBy<Guid>, ICommand
{
    public ISendTons[] Sends { get; set; }
}
