using System.Diagnostics;
using MediaForge.Core.Interfaces;

namespace MediaForge.Infrastructure.FFmpeg;

public sealed class ThumbnailService : IThumbnailService
{
    public async Task<string?> CreateThumbnailAsync(string videoPath)
    {
        string cacheFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MediaForge",
            "Cache",
            "Thumbnails");

        Directory.CreateDirectory(cacheFolder);

        string thumbnailPath = Path.Combine(
            cacheFolder,
            $"{Path.GetFileNameWithoutExtension(videoPath)}.jpg");

        string ffmpegPath = FFmpegLocator.GetFFmpegPath();

        ProcessStartInfo startInfo = new()
        {
            FileName = ffmpegPath,
            Arguments =
                $"-y -ss 1 -i \"{videoPath}\" -frames:v 1 \"{thumbnailPath}\"",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process? process = Process.Start(startInfo);

if (process is null)
    throw new Exception("Unable to start FFmpeg process.");

await process.WaitForExitAsync();

string output = await process.StandardOutput.ReadToEndAsync();
string error = await process.StandardError.ReadToEndAsync();

if (!File.Exists(thumbnailPath))
{
    throw new Exception(
$"""
FFmpeg failed.

Exit Code: {process.ExitCode}

FFmpeg Path:
{ffmpegPath}

Command:
{startInfo.Arguments}

Output:
{output}

Error:
{error}

Expected Thumbnail:
{thumbnailPath}
""");
}

return thumbnailPath;
    }
}