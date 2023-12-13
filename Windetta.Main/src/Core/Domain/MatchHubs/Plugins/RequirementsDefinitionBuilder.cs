using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Core.Domain.MatchHubs.Plugins;
public sealed class RequirementsDefinitionBuilder
{
    private readonly RequirementsDefinition _requirements;

    public RequirementsDefinitionBuilder()
    {
        _requirements = new();
    }

    /// <summary>
    /// Adds parametrized requirement
    /// </summary>
    /// <typeparam name="TTypeOfRequirement">Example : int, bool, string, DateTime etc. Only Built-in types allowed</typeparam>
    /// <param name="name">Name of requirement. Example: "minReputation", "requiredRating", "isAdmin"</param>
    /// <returns>builder</returns>
    public RequirementsDefinitionBuilder Add<TTypeOfRequirement>(string name)
    {
        var type = typeof(TTypeOfRequirement);

        if (!ContainsInAllowedTypes(type))
            throw MatchHubPluginException.RequirementTypeNotAllowed;

        _requirements.Add<TTypeOfRequirement>(name);

        return this;
    }

    public RequirementsDefinition Build()
    {
        _requirements.FinishToAdd();

        return _requirements;
    }

    private static bool ContainsInAllowedTypes(Type type)
    {
        return type.IsPrimitive
            || type.Equals(typeof(string))
            || type.Equals(typeof(DateTime))
            || type.Equals(typeof(DateTimeOffset));
    }
}
