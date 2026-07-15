using System.Windows;
using MediaForge.Services;
using MediaForge.ViewModels;

namespace MediaForge;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel(
            new FileDialogService());
    }
}