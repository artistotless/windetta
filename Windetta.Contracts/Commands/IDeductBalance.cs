using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface IDeductBalance : ICommand, CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public int CurrencyId { get; set; }
    public long Amount { get; set; }
}