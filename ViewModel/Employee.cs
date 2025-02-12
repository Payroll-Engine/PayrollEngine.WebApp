using System;
using System.Collections.Generic;
using System.Linq;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Employee : Client.Model.Employee, IViewModel,
    IViewAttributeObject, IKeyEquatable<Employee>
{
    public Employee()
    {
    }

    public Employee(Employee copySource) :
        base(copySource)
    {
    }

    public Employee(Client.Model.Employee copySource) :
        base(copySource)
    {
    }

    public string FullName => $"{FirstName} {LastName}";

    #region Attributes

    /// <inheritdoc />
    public string GetStringAttribute(string name) =>
        Attributes?.GetStringAttributeValue(name);

    /// <inheritdoc />
    public decimal GetNumericAttribute(string name) =>
        Attributes?.GetDecimalAttributeValue(name) ?? 0;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? false;

    #endregion

    public string DivisionsAsString
    {
        get
        {
            if (Divisions == null || !Divisions.Any())
            {
                return string.Empty;
            }
            return string.Join(',', Divisions);
        }
        set
        {
            Divisions ??= [];
            Divisions.Clear();
            if (value != null)
            {
                Divisions.AddRange(value.Split(',', StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }

    public IEnumerable<string> DivisionsAsEnum
    {
        get => Divisions;
        set => Divisions = value?.ToList();
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Employee compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Employee);

    public bool EqualKey(Employee compare) =>
        base.EqualKey(compare);

    public override string ToString() => FullName;
}