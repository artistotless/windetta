using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface ICreateUserWallet : ICommand, CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
}
