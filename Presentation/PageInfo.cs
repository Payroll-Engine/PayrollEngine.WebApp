using System;

namespace PayrollEngine.WebApp.Presentation;

public class PageInfo
{
    public Feature Feature { get; }
    public string PageLink { get; }
    public string PageName { get; }
    public string Title { get; }
    public PageGroupInfo PageGroup { get; }

    public PageInfo(Feature feature, string pageLink, string pageName,
        PageGroupInfo pageGroup = null) :
        this(feature, pageLink, pageName, pageName, pageGroup)
    {
    }

    private PageInfo(Feature feature, string pageLink, string pageName, string title,
        PageGroupInfo pageGroup = null)
    {
        if (string.IsNullOrWhiteSpace(pageLink))
        {
            throw new ArgumentException(nameof(pageLink));
        }
        if (string.IsNullOrWhiteSpace(pageName))
        {
            throw new ArgumentException(nameof(pageName));
        }
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(nameof(title));
        }

        if (!pageLink.EndsWith('/'))
        {
            pageLink += "/";
        }
        Feature = feature;
        PageLink = pageLink;
        PageName = pageName;
        Title = title;
        PageGroup = pageGroup;
    }

    public override string ToString() =>
        Enum.GetName(typeof(Feature), Feature);
}