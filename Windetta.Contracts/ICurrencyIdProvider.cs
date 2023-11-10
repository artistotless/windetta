using Windetta.Common.Types;

namespace Windetta.Contracts;

public interface ICurrencyIdProvider : ISingletonService
{
    public int Id { get; }
}
