﻿using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Payrun : Client.Model.Payrun, IViewModel, IKeyEquatable<Payrun>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Payrun()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Payrun(Payrun copySource) :
        base(copySource)
    {
        DivisionName = copySource.DivisionName;
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Payrun(Client.Model.Payrun copySource) :
        base(copySource)
    {
    }

    /// <summary>The division name</summary>
    private string DivisionName { get; }

    public string GetLocalizedDefaultReason(string culture) =>
        culture.GetLocalization(DefaultReasonLocalizations, DefaultReason);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Payrun compare) =>
        compare != null &&
        base.Equals(compare) &&
        string.Equals(DivisionName, compare.DivisionName);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Payrun);

    /// <inheritdoc />
    public bool EqualKey(Payrun compare) =>
        base.EqualKey(compare);
}