using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface ICreateUserWallet : CorrelatedBy<Guid>, ICommand
{
    public Guid UserId { get; set; }
}
