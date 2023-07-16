using Windetta.Common.Messages;

namespace Windetta.Common.Handlers;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query);
}