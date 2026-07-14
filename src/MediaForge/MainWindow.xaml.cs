using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace MediaForge;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenMedia_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new()
        {
            Title = "Open Media",
            Filter =
                "Media Files|*.mp4;*.mkv;*.avi;*.mov;*.mp3;*.wav;*.flac;*.aac|All Files|*.*"
        };

        if (dialog.ShowDialog() != true)
            return;

        FileInfo file = new(dialog.FileName);

        FileNameBox.Text = file.Name;
        FilePathBox.Text = file.FullName;
        FileSizeBox.Text = $"{file.Length:N0} bytes";

        StatusText.Text = "Media Loaded";
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}