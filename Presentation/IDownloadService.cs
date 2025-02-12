
namespace PayrollEngine.WebApp.Presentation;

/// <summary>Download service</summary>
public interface IDownloadService
{
    /// <summary>Max allowed download size</summary>
    long MaxAllowedSize { get; set; }
}