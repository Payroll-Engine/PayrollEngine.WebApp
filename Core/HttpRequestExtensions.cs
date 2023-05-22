using System.Net;
using System.Net.Http;

namespace PayrollEngine.WebApp;

public static class HttpRequestExtensions
{
    /// <summary>
    /// Checks for Error 400. Workaround implementation.
    /// </summary>
    /// <param name="exception">Thrown exception</param>
    /// <returns>Returns true if exception has http error code 400</returns>
    public static bool IsBadRequest(this HttpRequestException exception) => exception.Message.StartsWith(((int)HttpStatusCode.BadRequest).ToString());
}