using System.ComponentModel;
using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Core.MatchHubs.Plugins;

public abstract class ConfigurableMatchHubPlugin : IMatchHubPlugin
{
    public RequirementsDefinition? RequirementsDefinition { get; private init; }

    private IReadOnlyDictionary<string, string>? _requirementsValues;

    protected ConfigurableMatchHubPlugin()
    {
        RequirementsDefinition = SetupRequirements(
            new RequirementsDefinitionBuilder());
    }

    internal ConfigurableMatchHubPlugin WithValues(IDictionary<string, string> values)
    {
        if (RequirementsDefinition is null)
            return this;

        if (values is null)
            throw new ArgumentNullException(nameof(values));

        CleanRequirementsValues(values);
        ValidateRequirementsValues(values);

        _requirementsValues = values.AsReadOnly();

        return this;
    }

    protected T GetRequirementValue<T>(string nameOfRequirement)
    {
        object? result;

        try
        {
            result = TryGetRequirementValue<T>(nameOfRequirement);
        }
        catch
        {
            result = null;
        }

        if (result is null)
            throw MatchHubPluginException.RequirementValueInvalid;

        return (T)result;
    }

    protected abstract RequirementsDefinition? SetupRequirements(RequirementsDefinitionBuilder builder);

    private void CleanRequirementsValues(IDictionary<string, string> values)
    {
        foreach (var item in values)
        {
            if (!RequirementsDefinition!.Any(x => x.Name.Equals(item.Key)))
                values.Remove(item.Key);
        }
    }

    private void ValidateRequirementsValues(IDictionary<string, string> values)
    {
        if (!RequirementsDefinition!.All(x =>
        values.TryGetValue(x.Name, out var val) && !string.IsNullOrEmpty(val)))
            throw MatchHubPluginException.RequiredValuesNotProvided;
    }

    private object? TryGetRequirementValue<T>(string nameOfRequirement)
    {
        if (!_requirementsValues!.TryGetValue(nameOfRequirement, out var untypedValue))
            throw new Exception($"{nameOfRequirement} requirement not found");

        Type type = typeof(T);

        object? value;

        value = type switch
        {
            Type t when t.Equals(typeof(string)) => () => untypedValue,
            Type t when t.Equals(typeof(int)) => () => int.Parse(untypedValue),
            Type t when t.Equals(typeof(decimal)) => () => decimal.Parse(untypedValue),
            Type t when t.Equals(typeof(float)) => () => float.Parse(untypedValue),
            Type t when t.Equals(typeof(bool)) => () => bool.Parse(untypedValue),
            Type t when t.Equals(typeof(DateTime)) => () => DateTime.Parse(untypedValue),
            Type t when t.Equals(typeof(DateTimeOffset)) => () => DateTimeOffset.Parse(untypedValue),
            Type t when t.Equals(typeof(Guid)) => () => Guid.Parse(untypedValue),
            _ => null
        };

        if (value is not null)
            return value;

        TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
        value = typeConverter.ConvertFromString(untypedValue);

        return value;
    }
}