namespace LSPM.Core.Models;

public struct PortRange
{
    public int Start { get; init; }
    public int End { get; init; }

    public PortRange(int start, int end)
    {
        Start = start;
        End = end;
    }
}