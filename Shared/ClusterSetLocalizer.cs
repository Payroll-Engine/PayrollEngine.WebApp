using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ClusterSetLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string ClusterSet => PropertyValue();
    public string ClusterSets => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string IncludeClusters => PropertyValue();
    public string ExcludeClusters => PropertyValue();
}