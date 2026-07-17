using System;
using System.IO;

namespace MediaForge.Infrastructure.FFmpeg;

public static class FFmpegLocator
{
    public static string GetFFmpegPath()
    {
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "tools",
            "ffmpeg",
            "ffmpeg.exe");

        if (!File.Exists(path))
            throw new FileNotFoundException($"FFmpeg not found: {path}");

        return path;
    }

    public static string GetFFprobePath()
    {
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "tools",
            "ffmpeg",
            "ffprobe.exe");

        if (!File.Exists(path))
            throw new FileNotFoundException($"FFprobe not found: {path}");

        return path;
    }
}