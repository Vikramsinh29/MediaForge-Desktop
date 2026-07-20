using System;
using System.IO;
using System.Threading.Tasks;
using MediaForge.Core.Models;

namespace MediaForge.ViewModels;

public partial class MainWindowViewModel
{
    private async Task<ConversionResult> ConvertJobAsync(
        ConversionRequest request,
        IProgress<double> progress)
    {
        return await _conversionService.ConvertAsync(
            request,
            progress);
    }
}