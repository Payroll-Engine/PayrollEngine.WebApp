using System.Threading.Tasks;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>Item validator</summary>
public interface IItemValidator
{
    /// <summary>Validate item</summary>
    Task<bool> ValidateAsync();
}