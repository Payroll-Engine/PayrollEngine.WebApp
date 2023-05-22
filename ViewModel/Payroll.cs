using System;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Payroll : Client.Model.Payroll, IKeyEquatable<Payroll>
{
    public Payroll()
    {
    }

    public Payroll(Payroll copySource) :
        base(copySource)
    {
    }

    public Payroll(Client.Model.Payroll copySource) :
        base(copySource)
    {
    }

    public Country CountryType
    {
        get
        {
            // priority 1: ensure country
            if (CountryName.HasValue)
            {
                return CountryName.Value;
            }
            // priority 2: ensure country name
            if (Enum.IsDefined(typeof(Country), Country))
            {
                return (Country)Country;
            }
            return default;
        }
        set
        {
            Country = (int)value;
            CountryName = value;
        }
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Payroll compare) =>
        base.Equals(compare);

    public bool EqualKey(Payroll compare) =>
        base.EqualKey(compare);
}