namespace Windetta.Main.Core.Lobbies.Plugins;

public abstract class ConfigurableAutoDisposeStrategy : ConfigurableLobbyPlugin, IAutoDisposeStrategy
{
    public ILobbyDisposeListener Lobby { get; private set; }

    private Timer _timer;
    private readonly TimeSpan _checkInterval;

    protected ConfigurableAutoDisposeStrategy(TimeSpan? checkInterval = null)
    {
        _checkInterval = checkInterval ?? TimeSpan.FromSeconds(3);
    }

    public void Start(ILobbyDisposeListener lobby)
    {
        Lobby = lobby;

        if (_timer is null)
            _timer = new Timer(Update, null, TimeSpan.Zero, _checkInterval);
    }

    private void Update(object? state)
    {
        if (CheckDisposed())
        {
            _timer.Dispose();
            Lobby.CallDispose();
        }
    }

    protected abstract bool CheckDisposed();

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
