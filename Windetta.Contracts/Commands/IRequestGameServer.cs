using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface IRequestGameServer : ICommand, CorrelatedBy<Guid>
{
    public string LspmKey { get; set; }
}