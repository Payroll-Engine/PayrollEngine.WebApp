using System.Threading.Tasks;
using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

/// <summary>
/// Extension methods for <see cref="MudForm" />
/// </summary>
public static class MudFormExtensions
{
    /// <summary>
    /// Revalidate form
    /// </summary>
    /// <param name="form">Form to revalidate</param>
    public static async Task<bool> Revalidate(this MudForm form)
    {
        await form.ResetValidationAsync();
        await form.ValidateAsync();
        return form.IsValid;
    }
}