using System.IO;
using System.Diagnostics;
using System.Text;
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
        string arguments = FFmpegCommandBuilder.Build(request);
              

        ProcessStartInfo startInfo = new()
        {
            FileName = ffmpegPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = new()
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        StringBuilder stdout = new();
        StringBuilder stderr = new();


        FFmpegProgressParser parser = new(
            request.Duration,
            progress);

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is null)
                return;

            stdout.AppendLine(e.Data);

           
            Debug.WriteLine($"STDOUT: {e.Data}");

            parser.ProcessLine(e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is null)
                return;

            stderr.AppendLine(e.Data);

            
            Debug.WriteLine($"STDERR: {e.Data}");
        };

        process.Start();

        
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using CancellationTokenRegistration registration =
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

        await process.WaitForExitAsync(cancellationToken);
        process.WaitForExit();
        

        bool success =
            process.ExitCode == 0 &&
            File.Exists(request.OutputPath);

        return new ConversionResult
        {
            Success = success,
            ExitCode = process.ExitCode,
            OutputFile = success ? request.OutputPath : null,
            ErrorMessage = success ? null : stderr.ToString()
        };
    }
}