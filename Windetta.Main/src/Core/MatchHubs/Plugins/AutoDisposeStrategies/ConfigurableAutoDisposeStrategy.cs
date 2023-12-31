﻿namespace Windetta.Main.Core.MatchHubs.Plugins;

public abstract class ConfigurableAutoDisposeStrategy : ConfigurableMatchHubPlugin, IAutoDisposeStrategy
{
    public IHubDisposeListener Hub { get; private set; }

    private Timer _timer;
    private readonly TimeSpan _checkInterval;

    protected ConfigurableAutoDisposeStrategy(TimeSpan? checkInterval = null)
    {
        _checkInterval = checkInterval ?? TimeSpan.FromSeconds(3);
    }

    public void Start(IHubDisposeListener hub)
    {
        Hub = hub;

        if (_timer is null)
            _timer = new Timer(Update, null, TimeSpan.Zero, _checkInterval);
    }

    private void Update(object? state)
    {
        if (CheckDisposed())
        {
            _timer.Dispose();
            Hub.OnHubAutoDisposed();
        }
    }

    protected abstract bool CheckDisposed();

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
