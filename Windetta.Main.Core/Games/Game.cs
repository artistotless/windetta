namespace Windetta.Main.Core.Games;

/// <summary>
/// The domain model of the game
/// Is a reference object and is rarely changed
/// </summary>
public class Game
{
    /// <summary>
    /// Unique game identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the game
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Unique game code
    /// To display in ui page paths
    /// </summary>
    /// <remarks>For example: poker-holdem, battleship</remarks>
    public string Code { get; set; }

    /// <summary>
    /// Description of the game
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Path to the game icon
    /// </summary>
    public string IconPath { get; set; }

    /// <summary>
    /// Configuration of the number of teams and players of the game
    /// </summary>
    public GameConfiguration Configuration { get; set; }

    /// <summary>
    /// Currencies that can be used as a bet, their minimum and maximum values
    /// </summary>
    /// <remarks>
    /// If a player does not have a currency from this list - 
    /// he will not be able to create a match for this game
    /// </remarks>
    public IEnumerable<SupportedCurrency> SupportedCurrencies { get; set; }
}
