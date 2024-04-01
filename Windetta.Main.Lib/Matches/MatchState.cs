namespace Windetta.Main.Core.Matches;

/// <summary>
/// States in which a match may be in
/// </summary>
public enum MatchState
{
    Created,    // Just created
    Ongoing,    // In Progress
    Canceled,   // Canceled for some reason
    Completed   // Completed, there is a winner
}