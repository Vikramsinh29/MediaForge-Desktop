using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediaForge.Services.Batch.Contracts;
using MediaForge.Services.Batch.Models;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;
using MediaForge.ViewModels;
using MediaForge.Services.Batch.Utilities;

namespace MediaForge.Services.Batch;

/// <summary>
/// Executes batch conversions.
/// </summary>
public sealed class BatchConversionService : IBatchConversionService
{
    private readonly IConversionService _conversionService;
    private readonly OutputPathBuilder _outputPathBuilder;

    public BatchConversionService(
        IConversionService conversionService,
        OutputPathBuilder outputPathBuilder)
    {
        _conversionService = conversionService;
        _outputPathBuilder = outputPathBuilder;
    }
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

            job.Status = JobStatus.Converting;

            string outputPath = _outputPathBuilder.Build(
                job.Media.FullPath,
                options.OutputFolder,
                job.OutputFormat!.Extension);

            ConversionRequest request = new()
            {
                InputPath = job.Media.FullPath,
                OutputPath = outputPath,
                OutputFormat = job.OutputFormat.Extension.TrimStart('.'),
                OverwriteExisting = true,
                Duration = TimeSpan.FromSeconds(job.Media.Duration)
            };

            IProgress<double> fileProgress = new Progress<double>(value =>
            {
                job.Progress = value;

                progress?.Report(new BatchConversionProgress
                {
                    CurrentJob = i + 1,
                    TotalJobs = options.Jobs.Count,
                    CurrentFile = job.Media.FileName,
                    Percentage = value
                });
            });

            ConversionResult result = await _conversionService.ConvertAsync(
                request,
                fileProgress,
                cancellationToken);

            if (result.Success)
            {
                successful++;

                job.Progress = 100;
                job.Status = JobStatus.Completed;
                job.OutputPath = outputPath;
            }
            else
            {
                failed++;

                job.Status = JobStatus.Failed;
                job.ErrorMessage = result.ErrorMessage;

                if (!options.ContinueOnError)
                {
                    skipped = options.Jobs.Count - (i + 1);
                    break;
                }
            }
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