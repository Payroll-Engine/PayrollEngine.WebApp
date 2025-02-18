using MudBlazor;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.JSInterop;
using NPOI.XSSF.UserModel;
using PayrollEngine.IO;
using PayrollEngine.Data;
using PayrollEngine.Document;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Excel download tool
/// </summary>
public static class ExcelDownload
{
    /// <summary>
    /// Start excel download
    /// </summary>
    /// <typeparam name="TItem">Object type</typeparam>
    /// <param name="grid">Source data grid</param>
    /// <param name="items">Items to download</param>
    /// <param name="jsRuntime">Javascript runtime</param>
    /// <param name="name">Download name</param>
    public static async Task StartAsync<TItem>(MudDataGrid<TItem> grid, IList<TItem> items, IJSRuntime jsRuntime,
        string name = null)
    {
        // column properties
        var properties = grid.GetColumnProperties();
        if (!properties.Any())
        {
            return;
        }

        // convert items to data set
        name ??= typeof(TItem).Name;
        var dataSet = new System.Data.DataSet(name);
        var dataTable = items.ToSystemDataTable(name, includeRows: true, properties: properties);
        dataSet.Tables.Add(dataTable);

        // xlsx workbook
        using var workbook = new XSSFWorkbook();
        // import 
        workbook.Import(dataSet);

        // result
        using var resultStream = new MemoryStream();
        workbook.Write(resultStream, true);
        resultStream.Seek(0, SeekOrigin.Begin);

        var download = $"{name}_{FileTool.CurrentTimeStamp()}{FileExtensions.ExcelDocument}";
        await jsRuntime.SaveAs(download, resultStream.ToArray());
    }
}