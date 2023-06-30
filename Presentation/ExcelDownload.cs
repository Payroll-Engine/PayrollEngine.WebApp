using MudBlazor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;
using PayrollEngine.Data;
using PayrollEngine.Document;
using PayrollEngine.IO;
using Microsoft.JSInterop;

namespace PayrollEngine.WebApp.Presentation;

public static class ExcelDownload
{
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
        name ??= typeof(TItem).Name.EnsureEnd("s");
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