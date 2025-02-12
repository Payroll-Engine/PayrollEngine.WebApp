using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;

namespace PayrollEngine.WebApp.Server.Components.Shared;

public class UserPasswordService(IUserService userService) : IUserPasswordService
{
    private IUserService UserService { get; } = userService ?? throw new ArgumentNullException(nameof(userService));

    public async Task<bool> TestPasswordAsync(int tenantId, int userId, string password)
    {
        // initialize password check
        bool test;

        // test service
        try
        {
            test = await UserService.TestPasswordAsync(new(tenantId), userId, password);
        }
        catch (HttpRequestException httpException)
        {
            test = !IsBadRequest(httpException);
        }
        catch (Exception exception)
        {
            Log.Critical(exception, exception.GetBaseMessage());
            test = false;
        }

        return test;
    }

    private static bool IsBadRequest(HttpRequestException exception) =>
        exception.Message.StartsWith(((int)HttpStatusCode.BadRequest).ToString());

    public async Task<bool> ChangePasswordAsync(int tenantId, int userId, PasswordChangeRequest changeRequest)
    {
        // valid format
        if (!string.IsNullOrWhiteSpace(changeRequest.NewPassword) && !UserPassword.IsValid(changeRequest.NewPassword))
        {
            return false;
        }

        try
        {
            await UserService.UpdatePasswordAsync(new(tenantId), userId, changeRequest);
            return true;
        }
        catch (Exception exception)
        {
            Log.Critical(exception, exception.GetBaseMessage());
            return false;
        }
    }
}