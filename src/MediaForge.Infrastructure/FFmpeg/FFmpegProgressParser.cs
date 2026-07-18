using System.Globalization;

namespace MediaForge.Infrastructure.FFmpeg;

internal sealed class FFmpegProgressParser
{
    private readonly double _totalDurationMs;
    private readonly IProgress<double>? _progress;

    public FFmpegProgressParser(
        TimeSpan totalDuration,
        IProgress<double>? progress)
    {
        _totalDurationMs = totalDuration.TotalMilliseconds;
        _progress = progress;
    }

    public void ProcessLine(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        if (!line.StartsWith("out_time_ms="))
            return;

        if (!double.TryParse(
                line["out_time_ms=".Length..],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double currentMs))
        {
            return;
        }

        if (_totalDurationMs <= 0)
            return;

        double currentMilliseconds = currentMs / 1000.0;

        double percentage =
            currentMilliseconds / _totalDurationMs * 100.0;

        percentage = Math.Clamp(percentage, 0, 100);

        _progress?.Report(percentage);
    }
}