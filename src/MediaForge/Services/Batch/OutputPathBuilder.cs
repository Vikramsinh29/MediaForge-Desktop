using System.IO;

namespace MediaForge.Services.Batch;

/// <summary>
/// Builds output file paths for batch conversions.
/// </summary>
public sealed class OutputPathBuilder
{
    public string Build(
        string inputPath,
        string outputFolder,
        string extension)
    {
        string fileName = Path.GetFileNameWithoutExtension(inputPath);

        extension = extension.Trim();

        if (!extension.StartsWith('.'))
            extension = "." + extension;

        return Path.Combine(
            outputFolder,
            fileName + extension);
    }
}