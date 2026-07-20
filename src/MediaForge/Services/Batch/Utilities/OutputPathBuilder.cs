using System.IO;

namespace MediaForge.Services.Batch;

/// <summary>
/// Creates output file paths for batch conversions.
/// </summary>
public sealed class OutputPathBuilder
{
    public string Build(
        string inputPath,
        string outputFolder,
        string outputExtension)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            throw new ArgumentException("Input path is required.", nameof(inputPath));

        if (string.IsNullOrWhiteSpace(outputFolder))
            throw new ArgumentException("Output folder is required.", nameof(outputFolder));

        if (string.IsNullOrWhiteSpace(outputExtension))
            throw new ArgumentException("Output extension is required.", nameof(outputExtension));

        outputExtension = outputExtension.Trim();

        if (!outputExtension.StartsWith('.'))
            outputExtension = "." + outputExtension;

        string fileName = Path.GetFileNameWithoutExtension(inputPath);

        return Path.Combine(
            outputFolder,
            fileName + outputExtension);
    }
}