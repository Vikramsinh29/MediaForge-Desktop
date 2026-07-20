using System.Threading;
using System.Threading.Tasks;

namespace MediaForge.Services.Batch.Contracts;

public interface IBatchConversionService
{
    Task<BatchExecutionResult> ConvertAsync(
        BatchConversionOptions options,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);
}