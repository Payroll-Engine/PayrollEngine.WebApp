
namespace PayrollEngine.WebApp
{
    public class CultureDescription
    {
        public string Name { get; init; }
        public string DisplayName { get; init; }

        public override string ToString() =>
            $"{DisplayName} [{Name}]";
    }
}
