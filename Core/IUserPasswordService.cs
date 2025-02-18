using System.Threading.Tasks;

namespace PayrollEngine.WebApp;

/// <summary>
/// User password service
/// </summary>
public interface IUserPasswordService
{
    /// <summary>
    /// Check if password is correct or not (backend call)
    /// </summary>
    /// <returns>Returns true if password matches</returns>
    Task<bool> TestPasswordAsync(int tenantId, int userId, string password);

    /// <summary>
    /// Update the user password
    /// </summary>
    /// <returns>Returns true if password was changed</returns>
    Task<bool> ChangePasswordAsync(int tenantId, int userId, PasswordChangeRequest changeRequest);
}
