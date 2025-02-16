using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class DataGridPagerLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string AllItems => PropertyValue();
    public string FirstPage => PropertyValue();
    public string InfoFormat => PropertyValue();
    public string LastPage => PropertyValue();
    public string NextPage => PropertyValue();
    public string PreviousPage => PropertyValue();
    public string RowsPerPage => PropertyValue();
}