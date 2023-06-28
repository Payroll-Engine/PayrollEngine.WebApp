using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using MudBlazor;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation
{
    public class AppMudLocalizer : MudLocalizer
    {
        private static readonly Dictionary<string, string> LocalizationMap = new()
        {
            { "MudDataGrid.!=", nameof(DataGridLocalizer.SymbolNotEquals) },
            { "MudDataGrid.<", nameof(DataGridLocalizer.SymbolLessEquals) },
            { "MudDataGrid.<=", nameof(DataGridLocalizer.SymbolLessEquals) },
            { "MudDataGrid.=", nameof(DataGridLocalizer.SymbolEquals) },
            { "MudDataGrid.>", nameof(DataGridLocalizer.SymbolGreater) },
            { "MudDataGrid.>=", nameof(DataGridLocalizer.SymbolGreaterEquals) },
            { "MudDataGrid.AddFilter", nameof(DataGridLocalizer.AddFilter) },
            { "MudDataGrid.Apply", nameof(DataGridLocalizer.Apply) },
            { "MudDataGrid.Cancel", nameof(DataGridLocalizer.Cancel) },
            { "MudDataGrid.Clear", nameof(DataGridLocalizer.Clear) },
            { "MudDataGrid.CollapseAllGroups", nameof(DataGridLocalizer.CollapseAllGroups) },
            { "MudDataGrid.Column", nameof(DataGridLocalizer.Column) },
            { "MudDataGrid.Columns", nameof(DataGridLocalizer.Columns) },
            { "MudDataGrid.contains", nameof(DataGridLocalizer.Contains) },
            { "MudDataGrid.ends with", nameof(DataGridLocalizer.EndsWith) },
            { "MudDataGrid.equals", nameof(DataGridLocalizer.Equals) },
            { "MudDataGrid.ExpandAllGroups", nameof(DataGridLocalizer.ExpandAllGroups) },
            { "MudDataGrid.False", nameof(DataGridLocalizer.False) },
            { "MudDataGrid.Filter", nameof(DataGridLocalizer.Filter) },
            { "MudDataGrid.FilterValue", nameof(DataGridLocalizer.FilterValue) },
            { "MudDataGrid.Group", nameof(DataGridLocalizer.Group) },
            { "MudDataGrid.Hide", nameof(DataGridLocalizer.Hide) },
            { "MudDataGrid.HideAll", nameof(DataGridLocalizer.HideAll) },
            { "MudDataGrid.is", nameof(DataGridLocalizer.Is) },
            { "MudDataGrid.is after", nameof(DataGridLocalizer.IsAfter) },
            { "MudDataGrid.is before", nameof(DataGridLocalizer.IsBefore) },
            { "MudDataGrid.is empty", nameof(DataGridLocalizer.IsEmpty) },
            { "MudDataGrid.is not", nameof(DataGridLocalizer.IsNot) },
            { "MudDataGrid.is not empty", nameof(DataGridLocalizer.IsNotEmpty) },
            { "MudDataGrid.is on or after", nameof(DataGridLocalizer.IsOnOrAfter) },
            { "MudDataGrid.is on or before", nameof(DataGridLocalizer.IsOnOrBefore) },
            { "MudDataGrid.not contains", nameof(DataGridLocalizer.NotContains) },
            { "MudDataGrid.not equals", nameof(DataGridLocalizer.NotEquals) },
            { "MudDataGrid.Operator", nameof(DataGridLocalizer.Operator) },
            { "MudDataGrid.RefreshData", nameof(DataGridLocalizer.RefreshData) },
            { "MudDataGrid.Save", nameof(DataGridLocalizer.Save) },
            { "MudDataGrid.ShowAll", nameof(DataGridLocalizer.ShowAll) },
            { "MudDataGrid.starts with", nameof(DataGridLocalizer.StartsWith) },
            { "MudDataGrid.True", nameof(DataGridLocalizer.True) },
            { "MudDataGrid.Ungroup", nameof(DataGridLocalizer.Ungroup) },
            { "MudDataGrid.Unsort", nameof(DataGridLocalizer.Unsort) },
            { "MudDataGrid.Value", nameof(DataGridLocalizer.Value) }
        };

        private Localizer Localizer { get; }

        public AppMudLocalizer(Localizer localizer)
        {
            Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public override LocalizedString this[string key]
        {
            get
            {
                // unknown translation
                if (!LocalizationMap.ContainsKey(key))
                {
                    Log.Warning($"Unsupported MudBlazor translation key {key}");
                    return base[key];
                }

                var localizerKey = LocalizationMap[key];
                var value = Localizer.DataGrid.FromKey(localizerKey);
                return new(key, value);
            }
        }
    }
}
