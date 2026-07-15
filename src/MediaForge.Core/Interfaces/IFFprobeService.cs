using MediaForge.Core.Models;

namespace MediaForge.Core.Interfaces;

public interface IFFprobeService
{
    Task<MediaInfo> ReadAsync(string mediaFilePath);
}