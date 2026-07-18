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

        string ffmpegOptions =
    (request.OverwriteExisting ? "-y" : "-n") +
    " -progress pipe:1 -nostats";

        return request.OutputFormat.ToLowerInvariant() switch
        {
            // Audio
            "mp3" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -vn -c:a libmp3lame -b:a 320k \"{request.OutputPath}\"",

            "wav" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -vn -c:a pcm_s16le \"{request.OutputPath}\"",

            "flac" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -vn -c:a flac \"{request.OutputPath}\"",

            "aac" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -vn -c:a aac -b:a 192k \"{request.OutputPath}\"",

            "ogg" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -vn -c:a libvorbis -q:a 5 \"{request.OutputPath}\"",

            // Images
            "jpg" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -q:v 2 \"{request.OutputPath}\"",

            "jpeg" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -q:v 2 \"{request.OutputPath}\"",

            "png" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" \"{request.OutputPath}\"",

            "bmp" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" \"{request.OutputPath}\"",

            "webp" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -q:v 80 \"{request.OutputPath}\"",

            "tiff" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" \"{request.OutputPath}\"",    

            // Video
            "mp4" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -c:v libx264 -c:a aac \"{request.OutputPath}\"",

            "mkv" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -c:v libx264 -c:a aac \"{request.OutputPath}\"",

            "avi" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -c:v mpeg4 -c:a libmp3lame \"{request.OutputPath}\"",

            "mov" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -c:v libx264 -c:a aac \"{request.OutputPath}\"",

            "webm" =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" -c:v libvpx-vp9 -c:a libopus \"{request.OutputPath}\"",

            _ =>
                $"{ffmpegOptions} -i \"{request.InputPath}\" \"{request.OutputPath}\""
        };
    }
}