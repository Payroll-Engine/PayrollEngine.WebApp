using System;

namespace PayrollEngine.WebApp.ViewModel;

public class EmployeeCase : IEquatable<EmployeeCase>
{
    /// <summary>The employee identifier</summary>
    public string Identifier { get; set; }

    /// <summary>The first name of the employee</summary>
    public string FirstName { get; set; }

    /// <summary>The last name of the employee</summary>
    public string LastName { get; set; }

    /// <summary>Gets or sets the name of the case</summary>
    public string CaseName { get; set; }

    public EmployeeCase()
    {
    }

    public EmployeeCase(Client.Model.Employee copySource)
    {
        Identifier = copySource.Identifier;
        FirstName = copySource.FirstName;
        LastName = copySource.LastName;
    }

    public EmployeeCase(EmployeeCase copySource)
    {
        Identifier = copySource.Identifier;
        FirstName = copySource.FirstName;
        LastName = copySource.LastName;
        CaseName = copySource.CaseName;
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(EmployeeCase compare) =>
        compare != null &&
        string.Equals(Identifier, compare.Identifier) &&
        string.Equals(FirstName, compare.FirstName) &&
        string.Equals(LastName, compare.LastName) &&
        string.Equals(CaseName, compare.CaseName);
}