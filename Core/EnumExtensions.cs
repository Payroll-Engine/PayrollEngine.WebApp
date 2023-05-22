using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace PayrollEngine.WebApp;

public static class EnumExtensions
{
    /// <summary>
    /// Return EnumMemberValue (string) of Enum Value
    /// </summary>
    /// <typeparam name="T">Enum object</typeparam>
    /// <param name="value">Enum Member Value</param>
    /// <returns></returns>
    public static string GetMemberValue<T>(this T value) where T : struct, IConvertible
    {
        return typeof(T)
            .GetTypeInfo()
            .DeclaredMembers
            .SingleOrDefault(x => x.Name == value.ToString())
            ?.GetCustomAttribute<EnumMemberAttribute>(false)
            ?.Value;
    }
}