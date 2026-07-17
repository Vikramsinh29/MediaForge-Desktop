namespace MediaForge.Core.Interfaces;

public interface IThumbnailService
{
    Task<string?> CreateThumbnailAsync(string videoPath);
}