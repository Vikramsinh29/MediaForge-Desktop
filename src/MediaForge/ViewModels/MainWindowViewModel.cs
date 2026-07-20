using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MediaForge.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IFileDialogService _fileDialogService;
    private readonly IFFprobeService _ffprobeService;
    private readonly IThumbnailService _thumbnailService;
    private readonly IConversionService _conversionService;

    private MediaInfo? _currentMediaInfo;

    public MainWindowViewModel(
        IFileDialogService fileDialogService,
        IFFprobeService ffprobeService,
        IThumbnailService thumbnailService,
        IConversionService conversionService)
    {
        _fileDialogService = fileDialogService;
        _ffprobeService = ffprobeService;
        _thumbnailService = thumbnailService;
        _conversionService = conversionService;
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
    private async Task Open()
    {
        if (IsConverting)
            return;

        try
        {
           IReadOnlyList<MediaFile> files = _fileDialogService.PickMediaFiles();

            if (files.Count == 0)
                return;

            ConversionQueue.Clear();

            foreach (MediaFile file in files)
            {
                MediaInfo info = await _ffprobeService.ReadAsync(file.FullPath);

                OutputFormat? defaultFormat =
                    GetDefaultOutputFormat(Path.GetExtension(info.FullPath));

                ConversionQueue.Add(new ConversionJobViewModel
                {
                    Media = info,
                    OutputFormat = defaultFormat,
                    Status = JobStatus.Pending,
                    Progress = 0
                });
            }

            SelectedQueueItem = ConversionQueue.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    [RelayCommand]
    private async Task Convert()
    {
        if (IsConverting)
            return;

        try
        {
            if (SelectedQueueItem is null)
            {
                Status = "Please select a file.";
                return;
            }
                     

            if (SelectedOutputFormat is null)
{
    Status = "Please select an output format.";
    return;
}

string? outputPath = _fileDialogService.PickSaveFile(
    Path.GetFileNameWithoutExtension(SelectedQueueItem.Media.FileName),
    SelectedOutputFormat.Extension,
    SelectedOutputFormat.Filter);

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                Status = "Conversion cancelled.";
                return;
            }

            IsConverting = true;
            Status = "Converting...";

            Progress = 0;
            ProgressText = "0%";

            ConversionRequest request = new()
            {
                InputPath = SelectedQueueItem.Media.FullPath,
                OutputPath = outputPath,
                OutputFormat = SelectedOutputFormat.Extension.TrimStart('.'),
                OverwriteExisting = true,
                Duration = TimeSpan.FromSeconds(SelectedQueueItem.Media.Duration)
            };

            IProgress<double> conversionProgress = new Progress<double>(value =>
            {
                Progress = value;
                ProgressText = $"{value:F0}%";

                File.AppendAllText(
                    Path.Combine(AppContext.BaseDirectory, "ui-progress.log"),
                    $"{DateTime.Now:HH:mm:ss.fff}  {value:F2}%{Environment.NewLine}");
            });

            ConversionResult result =
                await _conversionService.ConvertAsync(
                    request,
                    conversionProgress);

            if (result.Success)
            {
                Progress = 100;
                ProgressText = "100%";
                Status = "Conversion completed.";
            }
            else
            {
                Status = result.ErrorMessage ?? "Conversion failed.";
            }
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
        finally
        {
            IsConverting = false;
        }
    }
    [RelayCommand]
    private void RemoveSelected()
    {
        if (SelectedQueueItem is null)
            return;

        int index = ConversionQueue.IndexOf(SelectedQueueItem);

        ConversionQueue.Remove(SelectedQueueItem);

        if (ConversionQueue.Count == 0)
        {
            SelectedQueueItem = null;

            _currentMediaInfo = null;

            PreviewImage = null;

            FileName = string.Empty;
            FullPath = string.Empty;
            FileSize = string.Empty;
            Container = string.Empty;
            VideoCodec = string.Empty;
            AudioCodec = string.Empty;
            Resolution = string.Empty;
            Duration = string.Empty;
            Fps = string.Empty;
            Bitrate = string.Empty;

            AvailableOutputFormats.Clear();
            SelectedOutputFormat = null;

            Status = "Queue empty.";

            return;
        }

        if (index >= ConversionQueue.Count)
            index = ConversionQueue.Count - 1;

        SelectedQueueItem = ConversionQueue[index];
    }

    [RelayCommand]
    private void ClearQueue()
    {
        ConversionQueue.Clear();

        SelectedQueueItem = null;

        _currentMediaInfo = null;

        PreviewImage = null;

        FileName = string.Empty;
        FullPath = string.Empty;
        FileSize = string.Empty;
        Container = string.Empty;
        VideoCodec = string.Empty;
        AudioCodec = string.Empty;
        Resolution = string.Empty;
        Duration = string.Empty;
        Fps = string.Empty;
        Bitrate = string.Empty;

        AvailableOutputFormats.Clear();
        SelectedOutputFormat = null;

        Status = "Queue cleared.";
    }    public async Task LoadAsync(MediaInfo info)
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