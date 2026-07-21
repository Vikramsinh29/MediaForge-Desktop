using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediaForge.Core.Interfaces;
using MediaForge.Core.Models;
using MediaForge.Services.Batch.Contracts;
using MediaForge.Services.Batch.Models;
using MediaForge.Services.Batch.Utilities;

namespace MediaForge.Services.Batch;

/// <summary>
/// Executes batch conversions.
/// </summary>
public sealed class BatchConversionService : IBatchConversionService
{
    private readonly IConversionService _conversionService;
    
    private readonly OutputPathBuilder _outputPathBuilder;

    private readonly BatchPauseController _pauseController;

    public BatchConversionService(
        IConversionService conversionService,
        OutputPathBuilder outputPathBuilder,
        BatchPauseController pauseController)
    {
        _conversionService = conversionService;
        _outputPathBuilder = outputPathBuilder;
        _pauseController = pauseController;
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

        Stopwatch stopwatch = Stopwatch.StartNew();

        int successful = 0;
        int failed = 0;
        int skipped = 0;

        for (int i = 0; i < options.Jobs.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _pauseController.WaitIfPaused(cancellationToken);

            var job = options.Jobs[i];
            
            progress?.Report(new BatchConversionProgress
            {
                CurrentJob = i + 1,
                TotalJobs = options.Jobs.Count,
                CurrentFile = job.Media.FileName,
                Percentage = ((double)i / options.Jobs.Count) * 100
            });

            try
            {
                job.Status = JobStatus.Converting;

                string outputPath =
                    _outputPathBuilder.Build(
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

                Progress<double> fileProgress = new(value =>
                {
                    job.Progress = value;

                    progress?.Report(new BatchConversionProgress
                    {
                        CurrentJob = i + 1,
                        TotalJobs = options.Jobs.Count,
                        CurrentFile = job.Media.FileName,
                        Percentage =
                            ((i + (value / 100.0)) / options.Jobs.Count) * 100.0
                    });
                });

                ConversionResult result =
                    await _conversionService.ConvertAsync(
                        request,
                        fileProgress,
                        cancellationToken);

                if (result.Success)
                {
                    job.Status = JobStatus.Completed;
                    job.OutputPath = result.OutputFile;
                    job.Progress = 100;

                    successful++;
                }
                else
                {
                    job.Status = JobStatus.Failed;
                    job.ErrorMessage = result.ErrorMessage;

                    failed++;

                    if (!options.ContinueOnError)
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                job.Status = JobStatus.Cancelled;
                job.Progress = 0;
                job.ErrorMessage = "Cancelled by user.";

                skipped++;

                break;
            }
            catch (Exception ex)
            {
                job.Status = JobStatus.Failed;
                job.ErrorMessage = ex.Message;

                failed++;

                if (!options.ContinueOnError)
                    break;
            }
        }

        stopwatch.Stop();

        progress?.Report(new BatchConversionProgress
        {
            CurrentJob = options.Jobs.Count,
            TotalJobs = options.Jobs.Count,
            CurrentFile = string.Empty,
            Percentage = 100
        });

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