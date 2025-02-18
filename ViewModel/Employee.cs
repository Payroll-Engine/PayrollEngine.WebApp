using System;
using System.Linq;
using System.Collections.Generic;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model employee
/// </summary>
public class Employee : Client.Model.Employee, IViewModel,
    IViewAttributeObject, IKeyEquatable<Employee>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Employee()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Employee(Employee copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Employee(Client.Model.Employee copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Full name
    /// </summary>
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

    /// <summary>
    /// Divisions as string
    /// </summary>
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

    /// <summary>
    /// Divisions as enum
    /// </summary>
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

    /// <inheritdoc />
    public override string ToString() => FullName;
}