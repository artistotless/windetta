namespace Windetta.Common.Messages;

public interface IQuery : IMessage { }
public interface IQuery<TResult> : IQuery { }
