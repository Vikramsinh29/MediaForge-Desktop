namespace MediaForge.Services.Batch.Models;

/// <summary>
/// Represents the current progress of a batch conversion.
/// </summary>
public sealed class BatchConversionProgress
{
    /// <summary>
    /// Current job number (1-based).
    /// </summary>
    public int CurrentJob { get; init; }

    /// <summary>
    /// Total number of jobs.
    /// </summary>
    public int TotalJobs { get; init; }

    /// <summary>
    /// File currently being converted.
    /// </summary>
    public string CurrentFile { get; init; } = string.Empty;

    /// <summary>
    /// Overall progress percentage (0–100).
    /// </summary>
    public double Percentage { get; init; }
}