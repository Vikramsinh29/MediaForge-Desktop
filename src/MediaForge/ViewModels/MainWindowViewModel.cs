using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaForge.Services;

namespace MediaForge.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly FileDialogService _dialog = new();

    public string Title => "MediaForge";

    [ObservableProperty]
    private string status = "Ready";

    [ObservableProperty]
    private string selectedFile = "No file selected";

    [RelayCommand]
    private void Open()
    {
        var media = _dialog.PickMediaFile();

        if (media is null)
            return;

        SelectedFile = media.FullPath;
        Status = "Media Loaded";
    }
}