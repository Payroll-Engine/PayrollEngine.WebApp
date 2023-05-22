using System;

namespace PayrollEngine.WebApp.ViewModel;

public class Log : Client.Model.Log, IViewModel, IEquatable<Log>
{
    public Log()
    {
    }

    public Log(Log copySource) :
        base(copySource)
    {
    }

    public Log(Client.Model.Log copySource) :
        base(copySource)
    {
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Log compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Log);
}
