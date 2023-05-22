using System;

namespace PayrollEngine.WebApp.Presentation;

public class PageGroupInfo
{
    public string GroupName { get; }
    public bool Expanded { get; set; }

    public PageGroupInfo(string groupName, bool expanded = false)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new ArgumentException(nameof(groupName));
        }

        GroupName = groupName;
        Expanded = expanded;
    }

    public override string ToString() => GroupName;
}