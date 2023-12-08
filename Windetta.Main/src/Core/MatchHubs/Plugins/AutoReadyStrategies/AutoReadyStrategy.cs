namespace Windetta.Main.Core.MatchHubs.Plugins;

public abstract class AutoReadyStrategy : IAutoReadyStrategy
{
    public IHubReadyListener Hub { get; private set; }

    private Timer _timer;
    private readonly TimeSpan _checkInterval;
     
    protected AutoReadyStrategy(TimeSpan? checkInterval = null)
    {
        _checkInterval = checkInterval ?? TimeSpan.FromSeconds(3);
    }

    public void Start(IHubReadyListener hub)
    {
        Hub = hub;

        if (_timer is null)
            _timer = new Timer(Update, null, _checkInterval, _checkInterval);
    }

    private void Update(object? state)
    {
        if (CheckReady())
        {
            _timer.Dispose();
            Hub.OnHubAutoReady();
        }
    }

    protected abstract bool CheckReady();

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
