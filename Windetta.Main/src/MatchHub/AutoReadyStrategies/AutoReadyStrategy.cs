namespace Windetta.Main.MatchHub.Strategies;

public abstract class AutoReadyStrategy : IDisposable
{
    public IHubReadyListener Hub { get; private set; }

    private Timer _timer;
    private readonly TimeSpan _checkInterval;

    protected AutoReadyStrategy(TimeSpan checkInterval)
    {
        _checkInterval = checkInterval;
    }

    public void Start(IHubReadyListener hub)
    {
        Hub = hub;

        if (_timer is null)
            _timer = new Timer(Update, null, TimeSpan.Zero, _checkInterval);
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
