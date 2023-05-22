using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace PayrollEngine.WebApp;

public static class JsRuntimeExtensions
{
    public static ValueTask Alert(this IJSRuntime jsRuntime, string message) =>
        jsRuntime.InvokeVoidAsync("alert", message);

    public static ValueTask<object> SaveAs(this IJSRuntime jsRuntime, string filename, byte[] data)
    {
        return jsRuntime.InvokeAsync<object>(
            "saveAsFile",
            filename,
            Convert.ToBase64String(data));
    }
}