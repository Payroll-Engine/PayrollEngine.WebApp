using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class DataGridPagerLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string AllItems => PropertyValue();
    public string FirstPage => PropertyValue();
    public string InfoFormat => PropertyValue();
    public string LastPage => PropertyValue();
    public string NextPage => PropertyValue();
    public string PreviousPage => PropertyValue();
    public string RowsPerPage => PropertyValue();
}