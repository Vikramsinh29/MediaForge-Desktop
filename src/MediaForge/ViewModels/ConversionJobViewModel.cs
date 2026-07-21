using CommunityToolkit.Mvvm.ComponentModel;
using MediaForge.Core.Models;

namespace MediaForge.ViewModels;

public partial class ConversionJobViewModel : ObservableObject
{
    [ObservableProperty]
    private MediaInfo media = default!;

    [ObservableProperty]
    private OutputFormat? outputFormat;

    [ObservableProperty]
    private JobStatus status = JobStatus.Pending;

    [ObservableProperty]
    private double progress;

    [ObservableProperty]
    private string? outputPath;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private TimeSpan? trimStart;

    partial void OnTrimStartChanged(TimeSpan? value)
    {
        if (TrimEnd.HasValue &&
            value.HasValue &&
            value >= TrimEnd)
        {
            TrimEnd = null;
        }
    }

    [ObservableProperty]
    private TimeSpan? trimEnd;

    partial void OnTrimEndChanged(TimeSpan? value)
    {
        if (TrimStart.HasValue &&
            value.HasValue &&
            value <= TrimStart)
        {
            TrimEnd = null;
        }
    }

    public bool HasTrim =>
        TrimStart.HasValue || TrimEnd.HasValue;

    [ObservableProperty]
    private VideoCompressionPreset compressionPreset =
        VideoCompressionPreset.None;
}