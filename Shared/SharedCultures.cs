using System.Collections.Generic;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared
{
    public static class SharedCultures
    {
        private static string[] SupportedCultures => new[]
        {
            "en",
            "de"
        };

        public static string DefaultCulture => "en-US";

        public static List<CultureInfo> GetCultures()
        {
            var cultures = new List<CultureInfo>();
            foreach (var culture in SupportedCultures)
            {
                cultures.Add(new(culture));
            }
            return cultures;
        }
    }
}
