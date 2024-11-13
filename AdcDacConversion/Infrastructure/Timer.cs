using System;
using Avalonia.Threading;

namespace AdcDacConversion.Infrastructure;

public class Timer
{
    public event Action<object?, EventArgs>? TimerElapsed;

    public double CurrentTime { get; private set; }

    private DispatcherTimer DpTimer { get; } = new();

    public Timer(TimeSpan interval)
    {
        DpTimer.Interval = interval;
        DpTimer.Tick += OnTimerElapsed;
    }

    private void OnTimerElapsed(object? sender, EventArgs e)
    {
        CurrentTime++;
        TimerElapsed?.Invoke(sender, e);
    }

    public void Start()
    {
        CurrentTime = 0;
        DpTimer.Start();
    }

    public void Stop() => DpTimer.Stop();

    public void Restart()
    {
        Stop();
        Start();
    }
}