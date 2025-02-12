using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Components.Shared;

public class DownloadService(long maxAllowedSize = 512000L) : IDownloadService
{
    /// <inheritdoc />
    public long MaxAllowedSize { get; set; } = maxAllowedSize;
}