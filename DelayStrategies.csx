using System;
using System.Threading;

public interface IDelayStrategy
{
    void Run();
}

public class NoDelayStrategy : IDelayStrategy
{
    public void Run()
    {
        // No delay added here!
    }
}

public class RandomDelayStrategy : IDelayStrategy
{
    private readonly TimeSpan _maxDelay;
    private readonly Random _random;
    public RandomDelayStrategy(TimeSpan maxDelay)
    {
        _random = new Random(Guid.NewGuid().GetHashCode());
        _maxDelay = maxDelay;
    }

    public void Run()
    {
       new ManualResetEvent(false).WaitOne(_random.Next((int)_maxDelay.TotalMilliseconds));
    }
}