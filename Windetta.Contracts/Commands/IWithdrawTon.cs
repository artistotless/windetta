using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface IWithdrawTon
{
    public Guid UserId { get; set; }
    public TonAddress Destination { get; set; }
    public long Nanotons { get; set; }
}
