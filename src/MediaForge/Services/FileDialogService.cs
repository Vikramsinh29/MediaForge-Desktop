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

    public string? PickSaveFile(
        string suggestedFileName,
        string defaultExtension,
        string filter)
    {
        SaveFileDialog dialog = new()
        {
            Title = "Save Converted Media",
            FileName = suggestedFileName,
            DefaultExt = defaultExtension,
            Filter = filter,
            AddExtension = true,
            OverwritePrompt = true
        };

        return dialog.ShowDialog() == true
            ? dialog.FileName
            : null;
    }
}