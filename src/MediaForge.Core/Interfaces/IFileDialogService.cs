using MediaForge.Core.Models;

namespace MediaForge.Core.Interfaces;

public interface IFileDialogService
{
    MediaFile? PickMediaFile();

    string? PickSaveFile(
        string suggestedFileName,
        string defaultExtension,
        string filter);
}