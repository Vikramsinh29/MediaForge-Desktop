using System.IO;
using Microsoft.Win32;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;

namespace MediaForge.Services;

public sealed class FileDialogService : IFileDialogService
{
    public MediaFile? PickMediaFile()
    {
        OpenFileDialog dialog = new()
        {
            Title = "Open Media",
            Filter = "Media Files|*.mp4;*.mkv;*.avi;*.mov;*.mp3;*.wav;*.flac;*.aac|All Files|*.*"
        };

        if (dialog.ShowDialog() != true)
            return null;

        FileInfo file = new(dialog.FileName);

        return new MediaFile
        {
            FileName = file.Name,
            FullPath = file.FullName,
            Size = file.Length
        };
    }
}