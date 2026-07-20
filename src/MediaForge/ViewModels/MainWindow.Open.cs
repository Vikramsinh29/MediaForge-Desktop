using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using MediaForge.Core.Models;

namespace MediaForge.ViewModels;

public partial class MainWindowViewModel
{
    [RelayCommand]
    private async Task Open()
    {
        if (IsConverting)
            return;

        try
        {
            IReadOnlyList<MediaFile> files = _fileDialogService.PickMediaFiles();

            if (files.Count == 0)
                return;

            ConversionQueue.Clear();

            foreach (MediaFile file in files)
            {
                MediaInfo info = await _ffprobeService.ReadAsync(file.FullPath);

                OutputFormat? defaultFormat =
                    GetDefaultOutputFormat(Path.GetExtension(info.FullPath));

                ConversionQueue.Add(new ConversionJobViewModel
                {
                    Media = info,
                    OutputFormat = defaultFormat,
                    Status = JobStatus.Pending,
                    Progress = 0
                });
            }

            SelectedQueueItem = ConversionQueue.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
    }
}