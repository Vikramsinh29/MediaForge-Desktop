using System.Diagnostics;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;

namespace MediaForge.Infrastructure.FFmpeg;

public sealed class ConversionService : IConversionService
{
    public async Task<ConversionResult> ConvertAsync(
        ConversionRequest request,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        string ffmpegPath = FFmpegLocator.GetFFmpegPath();

        ProcessStartInfo startInfo = new()
        {
            FileName = ffmpegPath,
            Arguments = FFmpegCommandBuilder.Build(request),
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process? process = Process.Start(startInfo);

        if (process is null)
        {
            return new ConversionResult
            {
                Success = false,
                ExitCode = -1,
                ErrorMessage = "Unable to start FFmpeg."
            };
        }

        cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                    process.Kill(true);
            }
            catch
            {
            }
        });

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);

        bool success =
            process.ExitCode == 0 &&
            File.Exists(request.OutputPath);

        progress?.Report(success ? 100 : 0);

        return new ConversionResult
        {
            Success = success,
            ExitCode = process.ExitCode,
            OutputFile = success ? request.OutputPath : null,
            ErrorMessage = success ? null : error
        };
    }
}