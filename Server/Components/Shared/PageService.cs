using System.Collections.Generic;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Components.Shared;

public class PageService(string baseLabel) : IPageService
{
    /// <inheritdoc />
    public string BaseLabel { get; } = baseLabel;

    /// <inheritdoc />
    public List<PageInfo> GetPages(Localizer localizer) =>
        new PageRegister(localizer).Pages;
}