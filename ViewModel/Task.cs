using System;
using System.Collections.Generic;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Task : Client.Model.Task, IViewModel,
    IViewAttributeObject, IKeyEquatable<Task>
{
    public Task()
    {
    }

    public Task(Task copySource) :
        base(copySource)
    {
    }

    public Task(Client.Model.Task copySource) :
        base(copySource)
    {
    }

    public new DateTime? Scheduled
    {
        get => IsScheduled ? base.Scheduled : null;
        set => base.Scheduled = value ?? DateTime.MinValue;
    }

    public bool IsScheduled => base.Scheduled != DateTime.MinValue;

    public string GetLocalizedName(Language language) =>
        language.GetLocalization(NameLocalizations, Name);

    public string InstructionText =>
        Instruction.RemoveLinks();

    public List<Tuple<string, string, string>> InstructionLinks =>
        Instruction.ExtractLinks();

    #region Attributes

    /// <inheritdoc />
    public string GetStringAttribute(string name) =>
        Attributes?.GetStringAttributeValue(name);

    /// <inheritdoc />
    public decimal GetNumericAttribute(string name) =>
        Attributes?.GetDecimalAttributeValue(name) ?? default;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? default;

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Task compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Task);

    public bool EqualKey(Task compare) => false;
}