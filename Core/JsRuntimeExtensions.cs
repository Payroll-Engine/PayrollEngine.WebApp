﻿using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace PayrollEngine.WebApp;

/// <summary>
/// Extension methods for <see cref="IJSRuntime" />
/// </summary>
public static class JsRuntimeExtensions
{
    /// <summary>
    /// Show javascript alert
    /// </summary>
    /// <param name="jsRuntime">Javascript runtime</param>
    /// <param name="message">Display message</param>
    public static ValueTask Alert(this IJSRuntime jsRuntime, string message) =>
        jsRuntime.InvokeVoidAsync("alert", message);

    /// <summary>
    /// Show save-as dialog
    /// </summary>
    /// <param name="jsRuntime">Javascript runtime</param>
    /// <param name="filename">Download file name</param>
    /// <param name="data">Download data</param>
    public static ValueTask<object> SaveAs(this IJSRuntime jsRuntime, string filename, byte[] data)
    {
        return jsRuntime.InvokeAsync<object>(
            "saveAsFile",
            filename,
            Convert.ToBase64String(data));
    }
}