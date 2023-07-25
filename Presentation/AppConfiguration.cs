using System;
using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace PayrollEngine.WebApp.Presentation;

/// <summary>The app configuration</summary>
public class AppConfiguration
{
    /// <summary>The application tile</summary>
    public string AppTitle { get; set; }

    /// <summary>The application image</summary>
    public string AppImage { get; set; }

    /// <summary>The application image in dark mode</summary>
    public string AppImageDarkMode { get; set; }

    /// <summary>The administrator contact</summary>
    public string AdminEmail { get; set; }

    /// <summary>The default features</summary>
    public List<string> DefaultFeatures { get; set; } = new();

    /// <summary>Log the case changes (default: false)</summary>
    public bool LogCaseChanges { get; set; }

    /// <summary>Session timeout in minutes (default: 10 minutes)</summary>
    public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>Maximum excel export count (default: 10'000)</summary>
    public int ExcelExportMaxRecords { get; set; } = 10000;

    #region Features

    // main
    public bool Tasks { get; set; }
    public bool EmployeeCases { get; set; }
    public bool CompanyCases { get; set; }
    public bool NationalCases { get; set; }
    public bool GlobalCases { get; set; }
    public bool Reports { get; set; }

    // payrun
    public bool PayrunResults { get; set; }
    public bool PayrunJobs { get; set; }
    public bool Payruns { get; set; }

    // payroll
    public bool Payrolls { get; set; }
    public bool PayrollLayers { get; set; }
    public bool Regulations { get; set; }
    public bool Regulation { get; set; }

    // administration
    public bool SharedRegulations { get; set; }
    public bool Tenants { get; set; }
    public bool Users { get; set; }
    public bool Calendars { get; set; }
    public bool Divisions { get; set; }
    public bool Employees { get; set; }
    public bool Webhooks { get; set; }
    public bool Logs { get; set; }

    // shared
    public bool Forecast { get; set; }

    // system
    public bool UserStorage { get; set; }

    public List<Feature> GetFeatures()
    {
        var features = new List<Feature>();

        // main
        AddFeature(Tasks, features, Feature.Tasks);
        AddFeature(EmployeeCases, features, Feature.EmployeeCases);
        AddFeature(CompanyCases, features, Feature.CompanyCases);
        AddFeature(NationalCases, features, Feature.NationalCases);
        AddFeature(GlobalCases, features, Feature.GlobalCases);
        AddFeature(Reports, features, Feature.Reports);

        // payrun
        AddFeature(PayrunResults, features, Feature.PayrunResults);
        AddFeature(PayrunJobs, features, Feature.PayrunJobs);
        AddFeature(Payruns, features, Feature.Payruns);

        // payroll
        AddFeature(Payrolls, features, Feature.Payrolls);
        AddFeature(PayrollLayers, features, Feature.PayrollLayers);
        AddFeature(Regulations, features, Feature.Regulations);
        AddFeature(Regulations, features, Feature.Regulation);

        // administration
        AddFeature(SharedRegulations, features, Feature.SharedRegulations);
        AddFeature(Tenants, features, Feature.Tenants);
        AddFeature(Tenants, features, Feature.Tenants);
        AddFeature(Users, features, Feature.Users);
        AddFeature(Calendars, features, Feature.Calendars);
        AddFeature(Divisions, features, Feature.Divisions);
        AddFeature(Employees, features, Feature.Employees);
        AddFeature(Webhooks, features, Feature.Webhooks);
        AddFeature(Logs, features, Feature.Logs);

        // shared
        AddFeature(Forecast, features, Feature.Forecasts);

        // system
        AddFeature(UserStorage, features, Feature.UserStorage);

        return features;
    }

    private static void AddFeature(bool add, List<Feature> features, Feature feature)
    {
        if (add)
        {
            features.Add(feature);
        }
    }

    #endregion
}