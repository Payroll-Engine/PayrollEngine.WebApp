﻿using System;
using System.Collections.Generic;
using System.Linq;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Payrun;

public class PayrunJobSetup
{
    /// <summary>
    /// The payrun time period
    /// </summary>
    public DateTime? Period { get; set; }

    /// <summary>
    /// Payrun job name
    /// </summary>
    public string JobName { get; set; }

    /// <summary>
    /// The forecast name
    /// </summary>
    public string ForecastName { get; set; }

    /// <summary>
    /// The payrun job reason
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    /// Selected employees, null=all
    /// </summary>
    public Employee SelectedEmployee { get; set; }

    /// <summary>
    /// The selected employees
    /// </summary>
    public IEnumerable<Employee> SelectedEmployees { get; set; }

    /// <summary>
    /// The payrun job parameters
    /// </summary>
    public List<PayrunParameter> Parameters { get; set; } = [];

    /// <summary>
    /// Test for valid legal payrun job
    /// </summary>
    /// <returns>True for a valid legal job</returns>
    public bool IsValidLegalJob() =>
        Period.HasValue &&
        !string.IsNullOrWhiteSpace(JobName) &&
        !string.IsNullOrWhiteSpace(Reason) &&
        ValidParameters();

    /// <summary>
    /// Test for valid forecast payrun job
    /// </summary>
    /// <returns>True for a valid forecast job</returns>
    public bool IsValidForecastJob() =>
        IsValidLegalJob() &&
        !string.IsNullOrWhiteSpace(ForecastName);

    /// <summary>
    /// Test for valid payrun job parameters
    /// </summary>
    /// <returns>True for a valid job parameters</returns>
    public bool ValidParameters()
    {
        var missing = Parameters?.Where(x => x.Mandatory &&
                                             string.IsNullOrWhiteSpace(x.Value)).ToList();
        return missing == null || missing.Count == 0;
    }
}