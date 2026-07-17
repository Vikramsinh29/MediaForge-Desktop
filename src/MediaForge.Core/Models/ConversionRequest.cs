namespace MediaForge.Core.Models;

public sealed class ConversionRequest
{
    public string InputPath { get; set; } = string.Empty;

    public string OutputPath { get; set; } = string.Empty;

    public string OutputFormat { get; set; } = string.Empty;

    public bool OverwriteExisting { get; set; }
}