using System.Threading.Tasks;
using MudBlazor;

namespace PayrollEngine.WebApp.Presentation;

public static class MudFormExtensions
{
    public static async Task<bool> Revalidate(this MudForm form)
    {
        form.ResetValidation();
        await form.Validate();
        return form.IsValid;
    }
}