using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediaForge.Services.Batch.Contracts;
using MediaForge.Services.Batch.Models;

namespace MediaForge.Services.Batch;

/// <summary>
/// Executes batch conversions.
/// </summary>
public sealed class BatchConversionService : IBatchConversionService
{
    public async Task<BatchExecutionResult> ConvertAsync(
        BatchConversionOptions options,
        IProgress<BatchConversionProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Jobs.Count == 0)
        {
            return new BatchExecutionResult
            {
                TotalJobs = 0,
                SuccessfulJobs = 0,
                FailedJobs = 0,
                SkippedJobs = 0,
                Elapsed = TimeSpan.Zero
            };
        }

        var stopwatch = Stopwatch.StartNew();

        int successful = 0;
        int failed = 0;
        int skipped = 0;

        for (int i = 0; i < options.Jobs.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var job = options.Jobs[i];

            progress?.Report(new BatchConversionProgress
            {
                CurrentJob = i + 1,
                TotalJobs = options.Jobs.Count,
                CurrentFile = job.Media.FileName,
                Percentage = ((double)(i + 1) / options.Jobs.Count) * 100.0
            });

            // TODO:
            // Replace this placeholder with the existing conversion pipeline.
            await Task.Yield();

            successful++;
        }

        stopwatch.Stop();

        return new BatchExecutionResult
        {
            TotalJobs = options.Jobs.Count,
            SuccessfulJobs = successful,
            FailedJobs = failed,
            SkippedJobs = skipped,
            Elapsed = stopwatch.Elapsed
        };
    }
}