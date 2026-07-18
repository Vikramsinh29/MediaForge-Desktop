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
        // Video
        new("MP4",  ".mp4",  "MP4 Files|*.mp4"),
        new("MKV",  ".mkv",  "MKV Files|*.mkv"),
        new("AVI",  ".avi",  "AVI Files|*.avi"),
        new("MOV",  ".mov",  "MOV Files|*.mov"),
        new("WEBM", ".webm", "WEBM Files|*.webm"),

        // Images
        new("JPG",  ".jpg",  "JPEG Files|*.jpg"),
        new("PNG",  ".png",  "PNG Files|*.png"),
        new("WEBP", ".webp", "WEBP Files|*.webp"),
        new("BMP",  ".bmp",  "Bitmap Files|*.bmp"),
        new("TIFF", ".tiff", "TIFF Files|*.tiff"),

        // Audio
        new("MP3",  ".mp3",  "MP3 Files|*.mp3"),
        new("WAV",  ".wav",  "WAV Files|*.wav"),
        new("FLAC", ".flac", "FLAC Files|*.flac"),
        new("AAC",  ".aac",  "AAC Files|*.aac"),
        new("OGG",  ".ogg",  "OGG Files|*.ogg")
    ];
}