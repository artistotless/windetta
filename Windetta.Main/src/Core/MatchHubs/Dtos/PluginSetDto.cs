namespace Windetta.Main.Core.MatchHubs.Dtos;

public class PluginSetDto
{
    public string Name { get; set; }
    public Dictionary<string, string>? RequirementsValues { get; set; }

    public PluginSetDto(string filterName, Dictionary<string, string>? requirementsValues = null)
    {
        Name = filterName;
        RequirementsValues = requirementsValues;
    }
}