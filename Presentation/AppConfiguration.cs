using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>The app configuration</summary>
public class AppConfiguration
{
    /// <summary>Culture (default: os working culture)</summary>
    public string Culture { get; set; }

    /// <summary>The application tile</summary>
    public string AppTitle { get; set; }

    /// <summary>The administrator contact</summary>
    public string AdminEmail { get; set; }

    /// <summary>The default features</summary>
    public List<string> DefaultFeatures { get; set; } = new();

    /// <summary>Log the case changes (default: false)</summary>
    public bool LogCaseChanges { get; set; }

    /// <summary>Session timeout in minutes (default: 10)</summary>
    public int SessionTimeout { get; set; } = DefaultSessionTimeout;

    /// <summary>The default session timeout inm minutes</summary>
    public static readonly int DefaultSessionTimeout = 10;

    /// <summary>Maximum excel export count (default: 10'000)</summary>
    public int ExcelExportMaxRecords { get; set; } = 10000;

    #region Features

    // Main
    public bool Tasks { get; set; }
    public bool EmployeeCases { get; set; }
    public bool CompanyCases { get; set; }
    public bool NationalCases { get; set; }
    public bool GlobalCases { get; set; }
    public bool Reports { get; set; }

    // Payrun
    public bool PayrunResults { get; set; }
    public bool PayrunJobs { get; set; }
    public bool Payruns { get; set; }

    // Payroll
    public bool Payrolls { get; set; }
    public bool PayrollLayers { get; set; }
    public bool Regulations { get; set; }
    public bool Regulation { get; set; }

    // Administration
    public bool SharedRegulations { get; set; }
    public bool Tenants { get; set; }
    public bool Users { get; set; }
    public bool Divisions { get; set; }
    public bool Employees { get; set; }
    public bool Logs { get; set; }

    // System
    public bool UserStorage { get; set; }

    public List<Feature> GetFeatures()
    {
        var features = new List<Feature>();

        // Main
        AddFeature(Tasks, features, Feature.Tasks);
        AddFeature(EmployeeCases, features, Feature.EmployeeCases);
        AddFeature(CompanyCases, features, Feature.CompanyCases);
        AddFeature(NationalCases, features, Feature.NationalCases);
        AddFeature(GlobalCases, features, Feature.GlobalCases);
        AddFeature(Reports, features, Feature.Reports);

        // Payrun
        AddFeature(PayrunResults, features, Feature.PayrunResults);
        AddFeature(PayrunJobs, features, Feature.PayrunJobs);
        AddFeature(Payruns, features, Feature.Payruns);

        // Payroll
        AddFeature(Payrolls, features, Feature.Payrolls);
        AddFeature(PayrollLayers, features, Feature.PayrollLayers);
        AddFeature(Regulations, features, Feature.Regulations);
        AddFeature(Regulations, features, Feature.Regulation);

        // Administration
        AddFeature(SharedRegulations, features, Feature.SharedRegulations);
        AddFeature(Tenants, features, Feature.Tenants);
        AddFeature(Tenants, features, Feature.Tenants);
        AddFeature(Users, features, Feature.Users);
        AddFeature(Divisions, features, Feature.Divisions);
        AddFeature(Employees, features, Feature.Employees);
        AddFeature(Logs, features, Feature.Logs);

        // System
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