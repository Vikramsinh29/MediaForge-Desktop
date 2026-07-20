namespace MediaForge.Services.Batch;

/// <summary>
/// Represents the outcome of a batch conversion operation.
/// </summary>
public sealed class BatchExecutionResult
{
    public int TotalJobs { get; init; }

    public int SuccessfulJobs { get; init; }

    public int FailedJobs { get; init; }

    public int SkippedJobs { get; init; }

    public TimeSpan Elapsed { get; init; }

    public bool Success =>
        FailedJobs == 0 &&
        SuccessfulJobs == TotalJobs &&
        TotalJobs > 0;
}