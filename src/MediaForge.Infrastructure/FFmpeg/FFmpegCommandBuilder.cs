using MediaForge.Core.Models;

namespace MediaForge.Infrastructure.FFmpeg;

internal static class FFmpegCommandBuilder
{
    public static string Build(ConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.InputPath))
            throw new ArgumentException("Input path is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.OutputPath))
            throw new ArgumentException("Output path is required.", nameof(request));

        string overwrite = request.OverwriteExisting ? "-y" : "-n";

        // Sprint 7.1
        // First real media conversion: MP4 -> MP3
        if (request.OutputFormat.Equals("mp3", StringComparison.OrdinalIgnoreCase))
        {
            return
                $"{overwrite} -i \"{request.InputPath}\" -vn -c:a libmp3lame -b:a 320k \"{request.OutputPath}\"";
        }

        // Existing behaviour
        return
            $"{overwrite} -i \"{request.InputPath}\" \"{request.OutputPath}\"";
    }
}