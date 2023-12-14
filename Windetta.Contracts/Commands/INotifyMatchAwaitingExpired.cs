using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface INotifyMatchAwaitingExpired : CorrelatedBy<Guid>, ICommand
{

}