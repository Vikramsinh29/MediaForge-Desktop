using System.Windows;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;
using MediaForge.Infrastructure.FFmpeg;
using MediaForge.Services;
using MediaForge.ViewModels;

namespace MediaForge;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private readonly IFFprobeService _ffprobeService;

    public MainWindow()
    {
        InitializeComponent();

        var fileDialogService = new FileDialogService();
        _ffprobeService = new FFprobeService();

        _viewModel = new MainWindowViewModel(
            fileDialogService,
            _ffprobeService);

        DataContext = _viewModel;
    }

    private void DropZone_DragEnter(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
            ? DragDropEffects.Copy
            : DragDropEffects.None;

        e.Handled = true;
    }

    private void DropZone_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
            ? DragDropEffects.Copy
            : DragDropEffects.None;

        e.Handled = true;
    }

    private async void DropZone_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        var files = (string[])e.Data.GetData(DataFormats.FileDrop)!;

        if (files.Length == 0)
            return;

        try
        {
            MediaInfo info = await _ffprobeService.ReadAsync(files[0]);
            _viewModel.Load(info);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "MediaForge",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}