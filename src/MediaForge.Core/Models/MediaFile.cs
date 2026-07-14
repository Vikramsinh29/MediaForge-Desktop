namespace MediaForge.Core.Models;

public sealed class MediaFile
{
    public string FileName { get; init; } = string.Empty;

    public string FullPath { get; init; } = string.Empty;

    public long Size { get; init; }
}