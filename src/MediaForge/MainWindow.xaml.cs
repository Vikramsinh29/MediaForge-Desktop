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

        IFileDialogService fileDialogService = new FileDialogService();

        _ffprobeService = new FFprobeService();

        IThumbnailService thumbnailService = new ThumbnailService();

        IConversionService conversionService = new ConversionService();

        _viewModel = new MainWindowViewModel(
            fileDialogService,
            _ffprobeService,
            thumbnailService,
            conversionService);

        DataContext = _viewModel;

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(
        object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.PreviewImage))
        {
            DropMessage.Visibility =
                _viewModel.PreviewImage is null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
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

            await _viewModel.LoadAsync(info);
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