using Windetta.Main.Core.MatchHubs.Plugins;

namespace Windetta.Main.MatchHubs.Strategies;

public abstract class ConfigurableAutoReadyStrategy : ConfigurableMatchHubPlugin, IAutoReadyStrategy
{
    public IHubReadyListener Hub { get; private set; }

    private Timer _timer;
    private readonly TimeSpan _checkInterval;

    protected ConfigurableAutoReadyStrategy(TimeSpan? checkInterval = null)
    {
        _checkInterval = checkInterval ?? TimeSpan.FromSeconds(3);
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
