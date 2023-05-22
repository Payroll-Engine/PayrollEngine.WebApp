﻿using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Pages;

public interface IReportOperator
{
    Task ShowReportLogAsync(Report report);
    Task StartReportAsync(Report report);
}