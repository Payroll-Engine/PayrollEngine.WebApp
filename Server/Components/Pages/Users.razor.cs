﻿using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Presentation.BackendService;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Users() : EditItemPageBase<ViewModel.User, Query, Dialogs.UserDialog>(WorkingItems.TenantChange) 
{
    [Inject]
    private UserBackendService UserBackendService { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Users);
    protected override IBackendService<ViewModel.User, Query> BackendService => UserBackendService;
    protected override ItemCollection<ViewModel.User> Items { get; } = new();
    protected override string GetLocalizedItemName(bool plural) => 
        plural ? Localizer.User.Users : Localizer.User.User;
}