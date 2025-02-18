using System;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Page info
/// </summary>
public class PageInfo
{
    //Page feature
    // ReSharper disable once MemberCanBePrivate.Global
    public Feature Feature { get; }

    /// <summary>
    /// Page link
    /// </summary>
    public string PageLink { get; }

    /// <summary>
    /// Page name
    /// </summary>
    public string PageName { get; }

    /// <summary>
    /// Page title
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Page group
    /// </summary>
    public PageGroupInfo PageGroup { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="feature">Page feature</param>
    /// <param name="pageLink">Page link</param>
    /// <param name="pageName">Page name</param>
    /// <param name="pageGroup">Page group</param>
    public PageInfo(Feature feature, string pageLink, string pageName,
        PageGroupInfo pageGroup = null) :
        this(feature, pageLink, pageName, pageName, pageGroup)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="feature">Page feature</param>
    /// <param name="pageLink">Page link</param>
    /// <param name="pageName">Page name</param>
    /// <param name="title">Page title</param>
    /// <param name="pageGroup">Page group</param>
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

        if (!pageLink.StartsWith('/'))
        {
            pageLink = "/" + pageLink;
        }

        Feature = feature;
        PageLink = pageLink;
        PageName = pageName;
        Title = title;
        PageGroup = pageGroup;
    }

    /// <inheritdoc />
    public override string ToString() =>
        Enum.GetName(typeof(Feature), Feature);
}