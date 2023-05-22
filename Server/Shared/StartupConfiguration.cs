
namespace PayrollEngine.WebApp.Server.Shared;

/// <summary>The web app startup configuration</summary>
public class StartupConfiguration
{
    /// <summary>Start tenant identifier</summary>
    public string StartupTenant { get; set; }

    /// <summary>Start user identifier</summary>
    public string StartupUser { get; set; }

    // <summary>Clear local blazor storage</summary>
    public bool ClearStorage { get; set; }

    // <summary>Dark mode</summary>
    public bool DarkMode { get; set; }

    // <summary>Automatic user login</summary>
    public bool AutoLogin { get; set; }
}