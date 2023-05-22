using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation;

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

    protected ItemCollection<ClusterSet> ClusterSets { get; set; } = new();
    protected MudDataGrid<ClusterSet> Grid { get; set; }

    #region Actions

    protected async Task AddClusterSetAsync()
    {
        // cluster set create dialog
        var dialog = await (await DialogService.ShowAsync<ClusterSetDialog>("Add cluster set")).Result;
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
        Payroll.ClusterSets ??= new();
        Payroll.ClusterSets.Add(item);
        ClusterSets.Add(item);
    }

    protected async Task EditClusterSetAsync(ClusterSet clusterSet)
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
        var dialog = await (await DialogService.ShowAsync<ClusterSetDialog>("Edit cluster set", parameters)).Result;
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

    protected async Task DeleteClusterSetAsync(ClusterSet clusterSet)
    {
        // existing
        if (!ClusterSets.Contains(clusterSet))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                "Delete cluster set",
                $"Delete {clusterSet.Name} permanently?"))
        {
            return;
        }

        // delete cluster set
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
