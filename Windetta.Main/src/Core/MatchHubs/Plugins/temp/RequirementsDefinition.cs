using System.Collections;

namespace Windetta.Main.Core.MatchHubs.Plugins;

public sealed class RequirementsDefinition : IEnumerable<RequirementItem>
{
    public bool IsFinishedToAdd { get; private set; }

    private readonly List<RequirementItem> _requirementItems;

    public RequirementsDefinition()
    {
        _requirementItems = new();
    }

    public void Add<T>(string name)
    {
        if (IsFinishedToAdd)
        {
            throw new InvalidOperationException(
                "Cannot add elements after build operation");
        }

        _requirementItems.Add(new RequirementItem(typeof(T).Name, name));
    }

    public void FinishToAdd()
        => IsFinishedToAdd = true;

    // TODO: implement it
    public override string ToString()
    {
        return base.ToString();
    }

    public IEnumerator<RequirementItem> GetEnumerator()
        => _requirementItems.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _requirementItems.GetEnumerator();
}
