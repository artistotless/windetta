using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface IWithdrawTon
{
    public Guid UserId { get; init; }
    public TonAddress Destination { get; init; }
    public long Nanotons { get; init; }
}
