namespace MediaForge.Core.Models;

public sealed class MediaInfo
{
    public string FileName { get; set; } = string.Empty;

    public string FullPath { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public string Container { get; set; } = string.Empty;

    public string VideoCodec { get; set; } = string.Empty;

    public string AudioCodec { get; set; } = string.Empty;

    public string Resolution { get; set; } = string.Empty;

    public double Duration { get; set; }

    public double FPS { get; set; }

    public long Bitrate { get; set; }
}