using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Dialogs;

public partial class PayrunParameterGrid : IDisposable
{
    [Parameter]
    public Tenant Tenant { get; set; }
    [Parameter]
    public Payrun Payrun { get; set; }
    [Parameter]
    public string Class { get; set; }
    [Parameter]
    public string Style { get; set; }
    [Parameter]
    public string Height { get; set; }

    [Inject]
    private IPayrunParameterService PayrunParameterService { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private IUserNotificationService UserNotification { get; set; }
    [Inject]
    private Localizer Localizer { get; set; }

    protected ItemCollection<PayrunParameter> PayrunParameters { get; set; } = new();
    protected MudDataGrid<PayrunParameter> Grid { get; set; }

    #region Actions

    protected async Task AddPayrunParameterAsync()
    {
        // payrun parameter create dialog
        var dialog = await (await DialogService.ShowAsync<PayrunParameterDialog>(
            Localizer.Item.AddTitle(Localizer.PayrunParameter.PayrunParameter))).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new payrun parameter
        if (dialog.Data is not PayrunParameter payrunParameter)
        {
            return;
        }

        // add payrun parameter
        try
        {
            var result = await PayrunParameterService.CreateAsync(new(Tenant.Id, Payrun.Id), payrunParameter);
            if (result != null)
            {
                PayrunParameters.Add(payrunParameter);
                await UserNotification.ShowSuccessAsync(Localizer.Item.Added(payrunParameter.Name));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Item.AddTitle(Localizer.PayrunParameter.PayrunParameter), exception);
        }
    }

    protected async Task EditPayrunParameterAsync(PayrunParameter payrunParameter)
    {
        // existing
        if (!PayrunParameters.Contains(payrunParameter))
        {
            return;
        }

        // edit copy
        var editItem = new PayrunParameter(payrunParameter);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(PayrunParameterDialog.PayrunParameter), editItem }
        };

        // payrun parameter edit dialog
        var dialog = await (await DialogService.ShowAsync<PayrunParameterDialog>(
            Localizer.Item.EditTitle(Localizer.PayrunParameter.PayrunParameter), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace payrun parameter
        try
        {
            await PayrunParameterService.UpdateAsync(new(Tenant.Id, Payrun.Id), editItem);
            PayrunParameters.Remove(payrunParameter);
            PayrunParameters.Add(editItem);
            await UserNotification.ShowSuccessAsync(Localizer.Item.Updated(payrunParameter.Name));
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Item.EditTitle(Localizer.PayrunParameter.PayrunParameter), exception);
        }
    }

    protected async Task DeletePayrunParameterAsync(PayrunParameter payrunParameter)
    {
        // existing
        if (!PayrunParameters.Contains(payrunParameter))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.DeleteTitle(Localizer.PayrunParameter.PayrunParameter),
                Localizer.Item.DeleteQuery(Localizer.PayrunParameter.PayrunParameter)))
        {
            return;
        }

        // delete payrun parameter
        try
        {
            await PayrunParameterService.DeleteAsync(new(Tenant.Id, Payrun.Id), payrunParameter.Id);
            PayrunParameters.Remove(payrunParameter);
            await UserNotification.ShowSuccessAsync(Localizer.Item.Removed(payrunParameter.Name));
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Item.DeleteTitle(Localizer.PayrunParameter.PayrunParameter), exception);
        }
    }

    #endregion

    #region Lifecycle

    private async Task SetupPayrunParameters()
    {
        PayrunParameters.Clear();

        // read payrun parameter
        try
        {
            var parameters = await PayrunParameterService.QueryAsync<PayrunParameter>(new(Tenant.Id, Payrun.Id),
                new() { Status = ObjectStatus.Active });
            foreach (var parameter in parameters)
            {
                PayrunParameters.Add(new(parameter));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Item.DeleteTitle(Localizer.PayrunParameter.PayrunParameter), exception);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Run(SetupPayrunParameters);
        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        PayrunParameters?.Dispose();
    }

    #endregion

}
