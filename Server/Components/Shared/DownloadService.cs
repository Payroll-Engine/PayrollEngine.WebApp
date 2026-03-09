using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Components.Shared;

/// <summary>
/// Download service with configurable size limit
/// </summary>
public class DownloadService(long maxAllowedSize = 512000L) : IDownloadService
{
    /// <inheritdoc />
    public long MaxAllowedSize { get; set; } = maxAllowedSize;
}