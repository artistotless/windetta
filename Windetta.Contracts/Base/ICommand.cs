using MassTransit;

namespace Windetta.Contracts.Base;

// marker interface
[ExcludeFromTopology]
public interface ICommand : IMessage { }
