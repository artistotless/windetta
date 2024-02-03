using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IBalanceRequested : IEvent
{
    public Guid UserId { get; set; }
    public int CurrencyId { get; set; }
}