namespace PayrollEngine.WebApp;

/// <summary>
/// Masked input
/// </summary>
public interface IMaskedInput
{
    /// <summary>
    /// Get the masked value
    /// </summary>
    /// <param name="maskedValue">MAsked value output</param>
    /// <returns>True for valid value</returns>
    bool TryGetMaskedValue(out string maskedValue);
}