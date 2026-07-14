using CommunityToolkit.Mvvm.ComponentModel;

namespace MediaForge.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string status = "Ready";
}