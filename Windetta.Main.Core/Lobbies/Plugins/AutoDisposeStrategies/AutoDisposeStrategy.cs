namespace Windetta.Main.Core.Lobbies.Plugins;

public abstract class AutoDisposeStrategy : IAutoDisposeStrategy
{
    public ILobbyDisposeListener Lobby { get; private set; }

    private Timer _timer;
    private readonly TimeSpan _checkInterval;

    protected AutoDisposeStrategy(TimeSpan? checkInterval = null)
    {
        _checkInterval = checkInterval ?? TimeSpan.FromSeconds(3);
    }

    public void Start(ILobbyDisposeListener lobby)
    {
        Lobby = lobby;

        if (_timer is null)
            _timer = new Timer(Update, null, _checkInterval, _checkInterval);
    }

    private void Update(object? state)
    {
        if (CheckDispose())
        {
            _timer.Dispose();
            Lobby.CallDispose();
        }
    }

    protected abstract bool CheckDispose();

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
