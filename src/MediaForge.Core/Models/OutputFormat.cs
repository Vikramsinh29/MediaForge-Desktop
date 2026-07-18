namespace MediaForge.Core.Models;

public sealed class OutputFormat
{
    public string Name { get; }

    public string Extension { get; }

    public string Filter { get; }


    public OutputFormat(
        string name,
        string extension,
        string filter)
    {
        Name = name;
        Extension = extension.StartsWith('.')
            ? extension
            : "." + extension;

        Filter = filter;
    }

    public override string ToString()
    {
        return Name;
    }

    public static IReadOnlyList<OutputFormat> DefaultFormats { get; } =
    [
        new("MP4", ".mp4", "MP4 Files|*.mp4"),
        new("MKV", ".mkv", "MKV Files|*.mkv"),
        new("AVI", ".avi", "AVI Files|*.avi"),
        new("MP3", ".mp3", "MP3 Files|*.mp3"),
        new("WAV", ".wav", "WAV Files|*.wav")
    ];
}