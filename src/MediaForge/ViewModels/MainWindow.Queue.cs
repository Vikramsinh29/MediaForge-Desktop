using CommunityToolkit.Mvvm.Input;

namespace MediaForge.ViewModels;

public partial class MainWindowViewModel
{
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
    }
}