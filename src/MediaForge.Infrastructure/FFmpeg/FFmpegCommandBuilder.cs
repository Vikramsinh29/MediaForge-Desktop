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

        return
            $"{overwrite} -i \"{request.InputPath}\" \"{request.OutputPath}\"";
    }
}