
namespace PayrollEngine.WebApp;

public interface IMaskedInput
{
    bool TryGetMaskedValue(out string maskedValue);
}