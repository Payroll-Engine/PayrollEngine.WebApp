using System.Collections.Generic;
using System.Linq;

namespace PayrollEngine.WebApp;

public static class EnumerableExtensions
{
    public static IEnumerable<T> GetOddEntries<T>(this IEnumerable<T> enumerable) => enumerable.Where((_, index) => index % 2 != 0);
    public static IEnumerable<T> GetEvenEntries<T>(this IEnumerable<T> enumerable) => enumerable.Where((_, index) => index % 2 == 0);
}