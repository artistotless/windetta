using Windetta.Common.Types;

namespace Windetta.Main.Infrastructure.Services;

public interface IInstanceIdProvider : ISingletonService
{
    string GetId();
}
