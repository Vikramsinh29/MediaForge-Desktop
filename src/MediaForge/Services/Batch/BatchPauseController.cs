using System.Threading;

namespace MediaForge.Services.Batch;

public sealed class BatchPauseController
{
    private readonly ManualResetEventSlim _pauseEvent = new(true);

    public bool IsPaused { get; private set; }

    public void Pause()
    {
        IsPaused = true;
        _pauseEvent.Reset();
    }

    public void Resume()
    {
        IsPaused = false;
        _pauseEvent.Set();
    }

    public void WaitIfPaused(CancellationToken cancellationToken)
    {
        _pauseEvent.Wait(cancellationToken);
    }
}