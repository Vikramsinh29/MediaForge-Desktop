using MediaForge.ViewModels;

namespace MediaForge.Services.Batch;

/// <summary>
/// Options controlling execution of a batch conversion.
/// </summary>
public sealed class BatchConversionOptions
{
    /// <summary>
    /// Jobs to process.
    /// </summary>
    public required IReadOnlyList<ConversionJobViewModel> Jobs { get; init; }

    /// <summary>
    /// Destination folder for converted files.
    /// </summary>
    public required string OutputFolder { get; init; }

    /// <summary>
    /// Continue processing remaining jobs if one fails.
    /// </summary>
    public bool ContinueOnError { get; init; } = true;
}