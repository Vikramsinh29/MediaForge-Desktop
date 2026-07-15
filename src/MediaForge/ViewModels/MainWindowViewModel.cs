using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;

namespace MediaForge.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IFileDialogService _fileDialogService;

    public MainWindowViewModel(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
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

    [RelayCommand]
    private void Open()
    {
        MediaFile? file = _fileDialogService.PickMediaFile();

        if (file is null)
            return;

        FileName = file.FileName;
        FullPath = file.FullPath;
        FileSize = $"{file.Size:N0} bytes";
        Status = "Media Loaded";
    }

    public void Load(MediaInfo info)
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

        Status = "Media Loaded";
    }
}