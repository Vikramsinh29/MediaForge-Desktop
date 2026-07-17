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

public MainWindowViewModel(
    IFileDialogService fileDialogService,
    IFFprobeService ffprobeService,
    IThumbnailService thumbnailService)
{
    _fileDialogService = fileDialogService;
    _ffprobeService = ffprobeService;
    _thumbnailService = thumbnailService;
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

    public async Task LoadAsync(MediaInfo info)
{
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
        string? thumbnail = await _thumbnailService.CreateThumbnailAsync(filePath);

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