using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface IUnDeductBalance : CorrelatedBy<Guid>, ICommand
{
}