using System;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Page group info
/// </summary>
public class PageGroupInfo
{
    /// <summary>
    /// Group name
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// Group expanded
    /// </summary>
    public bool Expanded { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="groupName">Group name</param>
    /// <param name="expanded">Expanded state</param>
    public PageGroupInfo(string groupName, bool expanded = false)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new ArgumentException(nameof(groupName));
        }

        GroupName = groupName;
        Expanded = expanded;
    }

    /// <inheritdoc />
    public override string ToString() => GroupName;
}