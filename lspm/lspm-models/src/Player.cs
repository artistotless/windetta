namespace LSPM.Models;

/// <summary>
/// Represents a player in the game with an ID, name, and team index.
/// </summary>
public class Player
{
    /// <summary>
    /// Gets or sets the unique identifier of the player.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the player.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the index of the team the player belongs to.
    /// </summary>
    public int TeamIndex { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// </summary>
    public Player()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class with the specified ID, name, and team index.
    /// </summary>
    /// <param name="id">The unique identifier of the player.</param>
    /// <param name="name">The name of the player.</param>
    /// <param name="teamIndex">The index of the team the player belongs to.</param>
    public Player(Guid id, string name, int teamIndex)
    {
        Id = id;
        Name = name;
        TeamIndex = teamIndex;
    }
}
