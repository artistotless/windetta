namespace Windetta.Main.Core.MatchHubs.Plugins;

public abstract class AutoDisposeStrategy : IAutoDisposeStrategy
{
    public IHubDisposeListener Hub { get; private set; }

    private Timer _timer;
    private readonly TimeSpan _checkInterval;

    protected AutoDisposeStrategy(TimeSpan? checkInterval = null)
    {
        _checkInterval = checkInterval ?? TimeSpan.FromSeconds(3);
    }

    public void Start(IHubDisposeListener hub)
    {
        Hub = hub;

        if (_timer is null)
            _timer = new Timer(Update, null, _checkInterval, _checkInterval);
    }

    private void Update(object? state)
    {
        if (CheckDispose())
        {
            _timer.Dispose();
            Hub.OnHubAutoDisposed();
        }
    }

    protected abstract bool CheckDispose();

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
