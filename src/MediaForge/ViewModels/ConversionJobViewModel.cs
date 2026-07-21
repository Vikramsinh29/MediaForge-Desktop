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

    [ObservableProperty]
    private TimeSpan? trimEnd;

    public bool HasTrim =>
        TrimStart.HasValue || TrimEnd.HasValue;
}