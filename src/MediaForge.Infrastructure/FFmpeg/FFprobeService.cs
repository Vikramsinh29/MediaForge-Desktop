using System.Diagnostics;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;

namespace MediaForge.Infrastructure.FFmpeg;

public sealed class FFprobeService : IFFprobeService
{
    public async Task<MediaInfo> ReadAsync(string mediaFilePath)
    {
        var ffprobePath = Path.Combine(
            AppContext.BaseDirectory,
            "tools",
            "ffmpeg",
            "ffprobe.exe");

        if (!File.Exists(ffprobePath))
        {
            throw new FileNotFoundException(
                "FFprobe executable was not found.",
                ffprobePath);
        }

        var arguments =
            $"-v quiet -print_format json -show_format -show_streams \"{mediaFilePath}\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = ffprobePath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);

        if (process is null)
            throw new InvalidOperationException("Unable to start FFprobe.");

        var json = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        // JSON parsing will be implemented next.
        return new MediaInfo
        {
            FileName = Path.GetFileName(mediaFilePath),
            FullPath = mediaFilePath,
            FileSize = new FileInfo(mediaFilePath).Length
        };
    }
}