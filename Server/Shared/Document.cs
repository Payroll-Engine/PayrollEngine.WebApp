using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace PayrollEngine.WebApp.Server.Shared;

/// <summary>
/// Set the document title using the current Uri
/// source: https://www.iambacon.co.uk/blog/setting-the-document-title-in-your-blazor-app
/// </summary>
public class Document : ComponentBase, IDisposable
{
    [Parameter]
    public string BaseLabel { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    private async Task SetTitle(Uri uri)
    {
        var name = uri.Segments.Last();
        await JsRuntime.InvokeVoidAsync("JsFunctions.setDocumentTitle", GetPageTitle(name, BaseLabel));
    }

    public static string GetPageTitle(string uri, string baseLabel)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new ArgumentException(nameof(uri));
        }

        // root page
        if ("/".Equals(uri))
        {
            return baseLabel;
        }

        // page register info
        var pageLink = uri.EnsureEnd("/");
        var page = PageRegister.Pages.FirstOrDefault(
            x => string.Equals(x.PageLink, pageLink, StringComparison.InvariantCultureIgnoreCase));

        // title
        var title = page == null ? uri : page.Title;
        return string.IsNullOrWhiteSpace(baseLabel) ? title : $"{baseLabel} - {title}";
    }

    private async void LocationChangedHandler(object sender, LocationChangedEventArgs e)
    {
        await SetTitle(new(e.Location));
    }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += LocationChangedHandler;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (!firstRender)
        {
            return;
        }
        await SetTitle(new(NavigationManager.Uri));
    }

    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            NavigationManager.LocationChanged -= LocationChangedHandler;
        }
    }
}