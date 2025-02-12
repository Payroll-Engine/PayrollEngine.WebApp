using PayrollEngine.WebApp.Shared;
using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>Page service</summary>
public interface IPageService
{
    /// <summary>Page base label</summary>
    string BaseLabel { get; }

    /// <summary>Get available pages</summary>
    List<PageInfo> GetPages(Localizer localizer);
}