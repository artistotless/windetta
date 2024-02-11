using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface ICreateUserWallet : ICommand
{
    public Guid UserId { get; set; }
}
