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

        string trimArguments = string.Empty;

        if (request.TrimStart.HasValue)
        {
            trimArguments += $" -ss {request.TrimStart.Value:hh\\:mm\\:ss\\.fff}";
        }

        if (request.TrimEnd.HasValue)
        {
            trimArguments += $" -to {request.TrimEnd.Value:hh\\:mm\\:ss\\.fff}";
        }

        return request.OutputFormat.ToLowerInvariant() switch
{
    // Audio
        "mp3" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -vn -c:a libmp3lame -b:a 320k \"{request.OutputPath}\"",

        "wav" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -vn -c:a pcm_s16le \"{request.OutputPath}\"",

        "flac" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -vn -c:a flac \"{request.OutputPath}\"",

        "aac" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -vn -c:a aac -b:a 192k \"{request.OutputPath}\"",

        "ogg" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -vn -c:a libvorbis -q:a 5 \"{request.OutputPath}\"",

        // Images
        "jpg" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -q:v 2 \"{request.OutputPath}\"",

        "jpeg" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -q:v 2 \"{request.OutputPath}\"",

        "png" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" \"{request.OutputPath}\"",

        "bmp" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" \"{request.OutputPath}\"",

        "webp" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -q:v 80 \"{request.OutputPath}\"",

        "tiff" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" \"{request.OutputPath}\"",

        // Video
        "mp4" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -c:v libx264 -c:a aac \"{request.OutputPath}\"",

        "mkv" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -c:v libx264 -c:a aac \"{request.OutputPath}\"",

        "avi" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -c:v mpeg4 -c:a libmp3lame \"{request.OutputPath}\"",

        "mov" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -c:v libx264 -c:a aac \"{request.OutputPath}\"",

        "webm" =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" -c:v libvpx-vp9 -c:a libopus \"{request.OutputPath}\"",

        _ =>
            $"{ffmpegOptions}{trimArguments} -i \"{request.InputPath}\" \"{request.OutputPath}\""
    };
    }
}