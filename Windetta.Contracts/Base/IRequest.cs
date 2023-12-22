using Windetta.Contracts.Base;

namespace Windetta.Common.Messages;

public interface IRequest<TResult> : IRequest { }
public interface IRequest : IMessage { }