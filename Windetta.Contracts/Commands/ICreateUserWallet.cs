using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface ICreateUserWallet : CorrelatedBy<Guid>, ICommand
{
    public Guid UserId { get; set; }
}
