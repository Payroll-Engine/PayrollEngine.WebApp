using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;

namespace PayrollEngine.WebApp.Server.Shared;

public class UserPasswordService : IUserPasswordService
{
    private IUserService UserService { get; }

    public UserPasswordService(IUserService userService)
    {
        UserService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public bool IsValidPassword(string test) =>
        ValidatePassword(test).Success;

    private static Match ValidatePassword(string test)
    {
        var expression = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$";
        var match = Regex.Match(test, expression);
        return match;
    }

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

    public async Task<bool> ChangePasswordAsync(int tenantId, int userId, string password)
    {
        // valid format
        if (!IsValidPassword(password))
        {
            return false;
        }

        try
        {
            await UserService.UpdatePasswordAsync(new(tenantId), userId, password);
            return true;
        }
        catch (Exception exception)
        {
            Log.Critical(exception, exception.GetBaseMessage());
            return false;
        }
    }
}