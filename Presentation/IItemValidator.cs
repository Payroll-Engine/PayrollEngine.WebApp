using System.Threading.Tasks;
using PayrollEngine.Client;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>Item validator</summary>
public interface IItemValidator<in T> where T : IModel
{
    /// <summary>Validate item</summary>
    Task<bool> ValidateAsync(T tenant);
}