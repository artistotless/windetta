using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface IUnDeductBalance : CorrelatedBy<Guid>, ICommand
{
}