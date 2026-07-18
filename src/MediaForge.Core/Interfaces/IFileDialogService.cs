using MediaForge.Core.Models;

namespace MediaForge.Core.Interfaces;

public interface IFileDialogService
{
    MediaFile? PickMediaFile();

    IReadOnlyList<MediaFile> PickMediaFiles();

    string? PickSaveFile(
        string suggestedFileName,
        string defaultExtension,
        string filter);
}