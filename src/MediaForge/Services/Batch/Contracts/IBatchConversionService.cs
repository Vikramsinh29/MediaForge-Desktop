using System;
using System.Threading;
using System.Threading.Tasks;
using MediaForge.Services.Batch.Models;

namespace MediaForge.Services.Batch.Contracts;

/// <summary>
/// Defines the contract for executing batch conversions.
/// </summary>
public interface IBatchConversionService
{
    /// <summary>
    /// Executes a batch conversion.
    /// </summary>
    Task<BatchExecutionResult> ConvertAsync(
        BatchConversionOptions options,
        IProgress<BatchConversionProgress>? progress = null,
        CancellationToken cancellationToken = default);
}
