using System;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model object
/// </summary>
public interface IViewModel : IEquatable<IViewModel>
{
    /// <summary>
    /// Model id
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// Model status
    /// </summary>
    ObjectStatus Status { get; set; }

    /// <summary>
    /// Model create date
    /// </summary>
    DateTime Created { get; set; }

    /// <summary>
    /// Model updated date
    /// </summary>
    DateTime Updated { get; set; }
}