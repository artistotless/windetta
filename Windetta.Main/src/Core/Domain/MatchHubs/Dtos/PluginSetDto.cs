namespace Windetta.Main.Core.Domain.MatchHubs.Dtos;

public class PluginSetDto
{
    public string Name { get; init; }
    public Dictionary<string, string>? RequirementsValues { get; init; }

    public PluginSetDto(string name, Dictionary<string, string>? requirementsValues = null)
    {
        Name = name;
        RequirementsValues = requirementsValues;
    }
}