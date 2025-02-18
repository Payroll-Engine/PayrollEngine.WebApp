using System.Globalization;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using MudBlazor;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// MudBlazor localization
/// </summary>
/// <param name="factory"></param>
/// <param name="userSession"></param>
public class AppMudLocalizer(IStringLocalizerFactory factory, UserSession userSession) : MudLocalizer
{
    private IStringLocalizerFactory Factory { get; } = factory;
    private UserSession UserSession { get; } = userSession;

    private static readonly Dictionary<string, string> LocalizationMap = new()
    {
        // data grid
        { "MudDataGrid_AddFilter", nameof(DataGridLocalizer.AddFilter) },
        { "MudDataGrid_Apply", nameof(DataGridLocalizer.Apply) },
        { "MudDataGrid_Cancel", nameof(DataGridLocalizer.Cancel) },
        { "MudDataGrid_Clear", nameof(DataGridLocalizer.Clear) },
        { "MudDataGrid_ClearFilter", nameof(DataGridLocalizer.ClearFilter) },
        { "MudDataGrid_CollapseAllGroups", nameof(DataGridLocalizer.CollapseAllGroups) },
        { "MudDataGrid_Column", nameof(DataGridLocalizer.Column) },
        { "MudDataGrid_Columns", nameof(DataGridLocalizer.Columns) },
        { "MudDataGrid_Contains", nameof(DataGridLocalizer.Contains) },
        { "MudDataGrid_EndsWith", nameof(DataGridLocalizer.EndsWith) },
        { "MudDataGrid_Equals", nameof(DataGridLocalizer.Equals) },
        { "MudDataGrid_EqualSign", nameof(DataGridLocalizer.SymbolEquals) },
        { "MudDataGrid_ExpandAllGroups", nameof(DataGridLocalizer.ExpandAllGroups) },
        { "MudDataGrid_False", nameof(DataGridLocalizer.False) },
        { "MudDataGrid_Filter", nameof(DataGridLocalizer.Filter) },
        { "MudDataGrid_FilterValue", nameof(DataGridLocalizer.FilterValue) },
        { "MudDataGrid_GreaterThanOrEqualSign", nameof(DataGridLocalizer.SymbolGreaterEquals) },
        { "MudDataGrid_GreaterThanSign", nameof(DataGridLocalizer.SymbolGreater) },
        { "MudDataGrid_Group", nameof(DataGridLocalizer.Group) },
        { "MudDataGrid_Hide", nameof(DataGridLocalizer.Hide) },
        { "MudDataGrid_HideAll", nameof(DataGridLocalizer.HideAll) },
        { "MudDataGrid_Is", nameof(DataGridLocalizer.Is) },
        { "MudDataGrid_IsAfter", nameof(DataGridLocalizer.IsAfter) },
        { "MudDataGrid_IsBefore", nameof(DataGridLocalizer.IsBefore) },
        { "MudDataGrid_IsEmpty", nameof(DataGridLocalizer.IsEmpty) },
        { "MudDataGrid_IsNot", nameof(DataGridLocalizer.IsNot) },
        { "MudDataGrid_IsNotEmpty", nameof(DataGridLocalizer.IsNotEmpty) },
        { "MudDataGrid_IsOnOrAfter", nameof(DataGridLocalizer.IsOnOrAfter) },
        { "MudDataGrid_IsOnOrBefore", nameof(DataGridLocalizer.IsOnOrBefore) },
        { "MudDataGrid_LessThanOrEqualSign", nameof(DataGridLocalizer.SymbolLessEquals) },
        { "MudDataGrid_LessThanSign", nameof(DataGridLocalizer.SymbolLessEquals) },
        { "MudDataGrid_Loading", nameof(DataGridLocalizer.Loading) },
        { "MudDataGrid_MoveDown", nameof(DataGridLocalizer.MoveDown) },
        { "MudDataGrid_MoveUp", nameof(DataGridLocalizer.MoveUp) },
        { "MudDataGrid_NotContains", nameof(DataGridLocalizer.NotContains) },
        { "MudDataGrid_NotEquals", nameof(DataGridLocalizer.NotEquals) },
        { "MudDataGrid_NotEqualSign", nameof(DataGridLocalizer.SymbolNotEquals) },
        { "MudDataGrid_OpenFilters", nameof(DataGridLocalizer.OpenFilters) },
        { "MudDataGrid_Operator", nameof(DataGridLocalizer.Operator) },
        { "MudDataGrid_RefreshData", nameof(DataGridLocalizer.RefreshData) },
        { "MudDataGrid_RemoveFilter", nameof(DataGridLocalizer.RemoveFilter) },
        { "MudDataGrid_Save", nameof(DataGridLocalizer.Save) },
        { "MudDataGrid_ShowAll", nameof(DataGridLocalizer.ShowAll) },
        { "MudDataGrid_ShowColumnOptions", nameof(DataGridLocalizer.ShowColumnOptions) },
        { "MudDataGrid_Sort", nameof(DataGridLocalizer.Sort) },
        { "MudDataGrid_StartsWith", nameof(DataGridLocalizer.StartsWith) },
        { "MudDataGrid_ToggleGroupExpansion", nameof(DataGridLocalizer.ToggleGroupExpansion) },
        { "MudDataGrid_True", nameof(DataGridLocalizer.True) },
        { "MudDataGrid_Ungroup", nameof(DataGridLocalizer.Ungroup) },
        { "MudDataGrid_Unsort", nameof(DataGridLocalizer.Unsort) },
        { "MudDataGrid_Value", nameof(DataGridLocalizer.Value) },

        // data grid pager
        { "MudDataGridPager_AllItems", nameof(DataGridPagerLocalizer.AllItems) },
        { "MudDataGridPager_FirstPage", nameof(DataGridPagerLocalizer.FirstPage) },
        { "MudDataGridPager_InfoFormat", nameof(DataGridPagerLocalizer.InfoFormat) },
        { "MudDataGridPager_LastPage", nameof(DataGridPagerLocalizer.LastPage) },
        { "MudDataGridPager_NextPage", nameof(DataGridPagerLocalizer.NextPage) },
        { "MudDataGridPager_PreviousPage", nameof(DataGridPagerLocalizer.PreviousPage) },
        { "MudDataGridPager_RowsPerPage", nameof(DataGridPagerLocalizer.RowsPerPage) },

        // input
        { "MudInput_Clear", nameof(InputLocalizer.Clear) },
        { "MudInput_Decrement", nameof(InputLocalizer.Increment) },
        { "MudInput_Increment", nameof(InputLocalizer.Decrement) },

        // date picker
        { "MudBaseDatePicker_NextMonth", nameof(DatePickerLocalizer.NextMonth) },
        { "MudBaseDatePicker_NextYear", nameof(DatePickerLocalizer.NextYear) },
        { "MudBaseDatePicker_PrevMonth", nameof(DatePickerLocalizer.PrevMonth) },
        { "MudBaseDatePicker_PrevYear", nameof(DatePickerLocalizer.PrevYear) },

        // nav group
        { "MudNavGroup_ToggleExpand", nameof(NavGroupLocalizer.ToggleExpand) },

        // snack bar
        { "MudSnackbar_Close", nameof(SnackBarLocalizer.Close) },
    };

    /// <inheritdoc />
    public override LocalizedString this[string key]
    {
        get
        {
            // unknown translation
            if (!LocalizationMap.TryGetValue(key, out var localizerKey))
            {
                Log.Warning($"Unsupported MudBlazor translation key {key}");
                return base[key];
            }

            var value = GetLocalizer().DataGrid.Key(localizerKey);
            return new(key, value);
        }
    }

    private Localizer defaultLocalizer;
    private Localizer localizer;
    private Localizer GetLocalizer()
    {
        if (localizer != null)
        {
            return localizer;
        }
        if (localizer == null && !string.IsNullOrEmpty(UserSession.User?.Culture))
        {
            localizer = new(Factory, new(UserSession.User.Culture));
            return localizer;
        }

        if (defaultLocalizer != null)
        {
            return defaultLocalizer;
        }

        defaultLocalizer = new(Factory, CultureInfo.CurrentUICulture);
        return defaultLocalizer;
    }

}