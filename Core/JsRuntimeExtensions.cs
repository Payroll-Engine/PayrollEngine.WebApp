using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace PayrollEngine.WebApp;

/// <summary>
/// Extension methods for <see cref="IJSRuntime" />
/// </summary>
public static class JsRuntimeExtensions
{
    /// <param name="jsRuntime">Javascript runtime</param>
    extension(IJSRuntime jsRuntime)
    {
        /// <summary>
        /// Show javascript alert
        /// </summary>
        /// <param name="message">Display message</param>
        public ValueTask Alert(string message) =>
            jsRuntime.InvokeVoidAsync("alert", message);

        /// <summary>
        /// Show save-as dialog
        /// </summary>
        /// <param name="filename">Download file name</param>
        /// <param name="data">Download data</param>
        public ValueTask<object> SaveAs(string filename, byte[] data)
        {
            return jsRuntime.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }
    }
}