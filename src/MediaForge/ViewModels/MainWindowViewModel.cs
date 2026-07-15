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
}