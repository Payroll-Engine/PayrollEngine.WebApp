using System;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model case change
/// </summary>
public class CaseChange : Client.Model.CaseChange, IViewModel, IEquatable<CaseChange>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public CaseChange()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public CaseChange(CaseChange copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public CaseChange(Client.Model.CaseChange copySource) :
        base(copySource)
    {
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(CaseChange compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as CaseChange);
}