using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public partial class ClusterSetGrid : IDisposable
{
    [Parameter]
    public Payroll Payroll { get; set; }
    [Parameter]
    public string Class { get; set; }
    [Parameter]
    public string Style { get; set; }
    [Parameter]
    public string Height { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private ItemCollection<ClusterSet> ClusterSets { get; set; } = new();
    private MudDataGrid<ClusterSet> Grid { get; set; }
    private Localizer Localizer => LocalizerService.Localizer;

    #region Actions

    private async Task AddClusterSetAsync()
    {
        // cluster set add dialog
        var dialog = await (await DialogService.ShowAsync<ClusterSetDialog>(
            Localizer.Item.AddTitle(Localizer.ClusterSet.ClusterSet))).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new cluster set
        if (dialog.Data is not ClusterSet item)
        {
            return;
        }

        // add cluster set
        Payroll.ClusterSets ??= [];
        Payroll.ClusterSets.Add(item);
        ClusterSets.Add(item);
    }

    private async Task EditClusterSetAsync(ClusterSet clusterSet)
    {
        // existing
        if (!ClusterSets.Contains(clusterSet))
        {
            return;
        }

        // edit copy
        var editItem = new ClusterSet(clusterSet);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(ClusterSetDialog.ClusterSet), editItem }
        };

        // cluster set edit dialog
        var dialog = await (await DialogService.ShowAsync<ClusterSetDialog>(
            Localizer.Item.EditTitle(Localizer.ClusterSet.ClusterSet), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace cluster set
        var index = Payroll.ClusterSets.IndexOf(clusterSet);
        Payroll.ClusterSets.RemoveAt(index);
        Payroll.ClusterSets.Insert(index, editItem);
        ClusterSets.Remove(clusterSet);
        ClusterSets.Add(editItem);
    }

    private async Task RemoveClusterSetAsync(ClusterSet clusterSet)
    {
        // existing
        if (!ClusterSets.Contains(clusterSet))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.RemoveTitle(Localizer.ClusterSet.ClusterSet),
                Localizer.Item.RemoveQuery(clusterSet.Name)))
        {
            return;
        }

        // remove cluster set
        Payroll.ClusterSets.Remove(clusterSet);
        ClusterSets.Remove(clusterSet);
    }

    #endregion

    #region Lifecycle

    private void SetupClusterSets()
    {
        ClusterSets.Clear();
        if (Payroll.ClusterSets == null)
        {
            return;
        }
        foreach (var clusterSet in Payroll.ClusterSets)
        {
            ClusterSets.Add(new(clusterSet));
        }
    }

    protected override async Task OnInitializedAsync()
    {
        SetupClusterSets();
        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        ClusterSets?.Dispose();
    }

    #endregion

}
