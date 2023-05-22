
namespace PayrollEngine.WebApp;

public interface IMaskedInput
{
    public bool TryGetMaskedValue(out string maskedValue);
}