namespace Windetta.Main.Core.Domain.Games;

public class Game
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public GameConfiguration Configuration { get; set; }
    public IEnumerable<SupportedCurrency> SupportedCurrencies { get; set; }
}
