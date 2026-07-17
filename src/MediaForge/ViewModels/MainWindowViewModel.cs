using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;

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

    [RelayCommand]
    private async Task Open()
    {
        try
        {
            MediaFile? file = _fileDialogService.PickMediaFile();

            if (file is null)
                return;

            MediaInfo info = await _ffprobeService.ReadAsync(file.FullPath);

            await LoadAsync(info);
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }

    [RelayCommand]
    private async Task Convert()
    {
        try
        {
            if (_currentMediaInfo is null)
            {
                Status = "Please open a media file first.";
                return;
            }

            string extension = Path.GetExtension(_currentMediaInfo.FullPath);

            string? outputPath = _fileDialogService.PickSaveFile(
                Path.GetFileNameWithoutExtension(_currentMediaInfo.FileName),
                extension,
                $"{extension.ToUpperInvariant()} Files|*{extension}");

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                Status = "Conversion cancelled.";
                return;
            }

            Status = "Converting...";

            ConversionRequest request = new()
            {
                InputPath = _currentMediaInfo.FullPath,
                OutputPath = outputPath,
                OutputFormat = extension.TrimStart('.'),
                OverwriteExisting = true
            };

            ConversionResult result =
                await _conversionService.ConvertAsync(request);

            if (result.Success)
            {
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
}