// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace PayrollEngine.WebApp.Server.Shared;

/// <summary>The web app startup configuration</summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class StartupConfiguration
{
    /// <summary>Start tenant identifier</summary>
    public string StartupTenant { get; set; }

    /// <summary>Start user identifier</summary>
    public string StartupUser { get; set; }

    // <summary>Clear local blazor storage</summary>
    public bool ClearStorage { get; set; }

    // <summary>Automatic user login</summary>
    public bool AutoLogin { get; set; }
}