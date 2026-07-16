using System.Diagnostics;
using System.Text.Json;
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

        using JsonDocument document = JsonDocument.Parse(json);

        JsonElement root = document.RootElement;

        JsonElement format = root.GetProperty("format");
        JsonElement streams = root.GetProperty("streams");

        var info = new MediaInfo
        {
            FileName = Path.GetFileName(mediaFilePath),
            FullPath = mediaFilePath,
            FileSize = new FileInfo(mediaFilePath).Length
        };

        if (format.TryGetProperty("format_name", out var container))
            info.Container = container.GetString() ?? string.Empty;

        if (format.TryGetProperty("duration", out var duration) &&
            double.TryParse(duration.GetString(), out var d))
            info.Duration = d;

        if (format.TryGetProperty("bit_rate", out var bitrate) &&
            long.TryParse(bitrate.GetString(), out var b))
            info.Bitrate = b;

        foreach (JsonElement stream in streams.EnumerateArray())
        {
            if (!stream.TryGetProperty("codec_type", out var codecType))
                continue;

            string? type = codecType.GetString();

            if (type == "video")
            {
                if (stream.TryGetProperty("codec_name", out var codec))
                    info.VideoCodec = codec.GetString() ?? string.Empty;

                int width = stream.GetProperty("width").GetInt32();
                int height = stream.GetProperty("height").GetInt32();

                info.Resolution = $"{width} x {height}";

                if (stream.TryGetProperty("r_frame_rate", out var fps))
                {
                    string value = fps.GetString() ?? "0/1";

                    string[] parts = value.Split('/');

                    if (parts.Length == 2 &&
                        double.TryParse(parts[0], out double num) &&
                        double.TryParse(parts[1], out double den) &&
                        den > 0)
                    {
                        info.FPS = num / den;
                    }
                }
            }

            if (type == "audio")
            {
                if (stream.TryGetProperty("codec_name", out var codec))
                    info.AudioCodec = codec.GetString() ?? string.Empty;
            }
        }

        return info;
    }
}