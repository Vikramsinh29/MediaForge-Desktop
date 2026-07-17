using MediaForge.Core.Models;

namespace MediaForge.Core.Interfaces;

public interface IConversionService
{
    Task<ConversionResult> ConvertAsync(
        ConversionRequest request,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);
}