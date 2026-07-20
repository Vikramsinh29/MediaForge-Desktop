using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;
using System.Collections.ObjectModel;
using MediaForge.Services.Batch.Contracts;
using MediaForge.Services.Batch;
using MediaForge.Services.Batch.Models;
using System.Windows;
using System.Linq;


namespace MediaForge.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IFFprobeService _ffprobeService;
    private readonly IThumbnailService _thumbnailService;
    private readonly IConversionService _conversionService;
    private readonly IBatchConversionService _batchConversionService;
    private MediaInfo? _currentMediaInfo;

    public MainWindowViewModel(
        IFileDialogService fileDialogService,
        IFFprobeService ffprobeService,
        IThumbnailService thumbnailService,
        IConversionService conversionService,
        IBatchConversionService batchConversionService)
    {
        _fileDialogService = fileDialogService;
        _ffprobeService = ffprobeService;
        _thumbnailService = thumbnailService;
        _conversionService = conversionService;
        _batchConversionService = batchConversionService;
    }

    [ObservableProperty]
    private string status = "Ready";

    [ObservableProperty]
    private string fileName = string.Empty;

    [ObservableProperty]
    private string fullPath = string.Empty;

    [ObservableProperty]
    private string fileSize = string.Empty;

    [ObservableProperty]
    private string container = string.Empty;

    [ObservableProperty]
    private string videoCodec = string.Empty;

    [ObservableProperty]
    private string audioCodec = string.Empty;

    [ObservableProperty]
    private string resolution = string.Empty;

    [ObservableProperty]
    private string duration = string.Empty;

    [ObservableProperty]
    private string fps = string.Empty;

    [ObservableProperty]
    private string bitrate = string.Empty;

    [ObservableProperty]
    private ImageSource? previewImage;

    [ObservableProperty]
    private bool isConverting;
    [ObservableProperty]
    private double progress;

    [ObservableProperty]
    private string progressText = "0%";

    [ObservableProperty]
    private ObservableCollection<OutputFormat> availableOutputFormats = [];

    [ObservableProperty]
    private OutputFormat? selectedOutputFormat;
    partial void OnSelectedOutputFormatChanged(OutputFormat? value)
    {
        if (SelectedQueueItem is null || value is null)
            return;

        SelectedQueueItem.OutputFormat = value;
    }

    [ObservableProperty]
    private ObservableCollection<ConversionJobViewModel> conversionQueue = [];

    [ObservableProperty]
    private ConversionJobViewModel? selectedQueueItem;

    partial void OnSelectedQueueItemChanged(ConversionJobViewModel? value)
    {
        if (value is null)
            return;

        _ = LoadAsync(value.Media);
    }
    
    [RelayCommand]
    private async Task Convert()
    {
        if (IsConverting)
            return;

        if (ConversionQueue.Count == 0)
        {
            Status = "No files in queue.";
            return;
        }

        string? outputFolder = _fileDialogService.PickFolder();

        if (string.IsNullOrWhiteSpace(outputFolder))
        {
            Status = "Conversion cancelled.";
            return;
        }

        try
        {
            IsConverting = true;
            Progress = 0;
            ProgressText = "0%";

            BatchConversionOptions options = new()
            {
                Jobs = ConversionQueue.ToList(),
                OutputFolder = outputFolder,
                ContinueOnError = true
            };

            Progress<BatchConversionProgress> progress =
                new(batchProgress =>
                {
                    Progress = batchProgress.Percentage;
                    ProgressText = $"{batchProgress.Percentage:F0}%";

                    Status =
                        $"Converting {batchProgress.CurrentJob} of {batchProgress.TotalJobs}: {batchProgress.CurrentFile}";
                });

            BatchExecutionResult result =
                await _batchConversionService.ConvertAsync(
                    options,
                    progress);

            Status =
                $"Completed. Success: {result.SuccessfulJobs}, Failed: {result.FailedJobs}, Skipped: {result.SkippedJobs}";

            Progress = 100;
            ProgressText = "100%";
            
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Conversion Error");

            Status = ex.Message;
        }
        finally
        {
            IsConverting = false;
        }
    }
    public async Task LoadAsync(MediaInfo info)
    {
        _currentMediaInfo = info;

        FileName = info.FileName;
        FullPath = info.FullPath;
        FileSize = $"{info.FileSize:N0} bytes";

        Container = info.Container;
        VideoCodec = info.VideoCodec;
        AudioCodec = info.AudioCodec;
        Resolution = info.Resolution;
        Duration = info.Duration.ToString("F2");
        Fps = info.FPS.ToString("F2");
        Bitrate = info.Bitrate.ToString();

        await LoadPreviewAsync(info.FullPath);

        AvailableOutputFormats.Clear();

string extension = Path.GetExtension(info.FullPath).ToLowerInvariant();

bool isVideo = extension is
    ".mp4" or ".mkv" or ".avi" or ".mov" or ".webm" or ".wmv" or ".flv" or ".m4v";

bool isAudio = extension is
    ".mp3" or ".wav" or ".flac" or ".aac" or ".ogg" or ".m4a" or ".wma";

bool isImage = extension is
    ".jpg" or ".jpeg" or ".png" or ".bmp" or ".webp" or ".tiff" or ".tif";

if (isVideo)
{
    foreach (OutputFormat format in OutputFormat.DefaultFormats)
    {
        if (format.Name is "MP4" or "MKV" or "AVI" or "MOV" or "WEBM")
        {
            AvailableOutputFormats.Add(format);
        }
    }
}
else if (isAudio)
{
    foreach (OutputFormat format in OutputFormat.DefaultFormats)
    {
        if (format.Name is "MP3" or "WAV" or "FLAC" or "AAC" or "OGG")
        {
            AvailableOutputFormats.Add(format);
        }
    }
}
else if (isImage)
{
    foreach (OutputFormat format in OutputFormat.DefaultFormats)
    {
        if (format.Name is "JPG" or "PNG" or "WEBP" or "BMP" or "TIFF")
        {
            AvailableOutputFormats.Add(format);
        }
    }
}

    SelectedOutputFormat = SelectedQueueItem?.OutputFormat
                       ?? AvailableOutputFormats.FirstOrDefault();

    Status = "Media Loaded";
    }

    private async Task LoadPreviewAsync(string filePath)
{
    PreviewImage = null;

    string extension = Path.GetExtension(filePath).ToLowerInvariant();

    // Image preview
    if (extension is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif")
    {
        BitmapImage image = new();

        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(filePath);
        image.EndInit();
        image.Freeze();

        PreviewImage = image;
        return;
    }

    // Video preview
    if (extension is ".mp4" or ".mkv" or ".avi" or ".mov"
        or ".wmv" or ".webm" or ".flv" or ".m4v")
    {
        string? thumbnail =
            await _thumbnailService.CreateThumbnailAsync(filePath);

        if (thumbnail is null)
            return;

        BitmapImage image = new();

        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(thumbnail);
        image.EndInit();
        image.Freeze();

        PreviewImage = image;
    }
}
private static bool IsVideo(string extension) =>
    extension is ".mp4" or ".mkv" or ".avi" or ".mov"
    or ".webm" or ".wmv" or ".flv" or ".m4v";

private static bool IsAudio(string extension) =>
    extension is ".mp3" or ".wav" or ".flac" or ".aac"
    or ".ogg" or ".m4a" or ".wma";

private static bool IsImage(string extension) =>
    extension is ".jpg" or ".jpeg" or ".png"
    or ".bmp" or ".webp" or ".tiff" or ".tif";

private static OutputFormat? GetDefaultOutputFormat(string extension)
{
    extension = extension.ToLowerInvariant();

    if (IsVideo(extension))
        return OutputFormat.DefaultFormats.FirstOrDefault(f => f.Name == "MP4");

    if (IsAudio(extension))
        return OutputFormat.DefaultFormats.FirstOrDefault(f => f.Name == "MP3");

    if (IsImage(extension))
        return OutputFormat.DefaultFormats.FirstOrDefault(f => f.Name == "JPG");

    return null;
}
}