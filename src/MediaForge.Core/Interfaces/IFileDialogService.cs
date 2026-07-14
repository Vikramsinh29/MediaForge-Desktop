using MediaForge.Core.Models;

namespace MediaForge.Core.Interfaces;

public interface IFileDialogService
{
    MediaFile? PickMediaFile();
}