namespace Windetta.Main.Core.MatchHubs.Plugins;

public struct RequirementItem
{
    public string Type { get; init; }
    public string Name { get; init; }

    public RequirementItem(string type, string name)
    {
        Type = type;
        Name = name;
    }
}