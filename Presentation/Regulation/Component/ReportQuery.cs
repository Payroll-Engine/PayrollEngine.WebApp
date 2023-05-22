using System.ComponentModel.DataAnnotations;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public class ReportQuery
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Value { get; set; }

    public ReportQuery()
    {
    }

    public ReportQuery(ReportQuery copySource)
    {
        Name = copySource.Name;
        Value = copySource.Value;
    }

    public ReportQuery(string name, string value)
    {
        Name = name;
        Value = value;
    }
}