using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayrollEngine.WebApp;

/// <summary>
/// Default JSON serializer
/// </summary>
public static class DefaultJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyProperties = true,
        WriteIndented = true
    };

    /// <summary>
    /// Deserialize object using the default options
    /// </summary>
    /// <param name="json">The JSON text</param>
    /// <typeparam name="T">The object type</typeparam>
    /// <returns>Object of type T</returns>
    public static T Deserialize<T>(string json) where T : class =>
        JsonSerializer.Deserialize<T>(json, Options);

    /// <summary>
    /// Serialize object using the default options
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <returns>The serialized JSON text</returns>
    public static string Serialize(object obj) =>
        JsonSerializer.Serialize(obj, Options);

    /// <summary>
    /// Serialize object using the default options
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <typeparam name="T">The object type</typeparam>
    /// <returns>The serialized JSON text</returns>
    public static string Serialize<T>(T obj) where T : class =>
        JsonSerializer.Serialize(obj, Options);

    /// <summary>
    /// Serialize object to string content using the default options
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <typeparam name="T">The object type</typeparam>
    /// <returns>The string content</returns>
    public static StringContent SerializeStringContent<T>(T obj) where T : class =>
        SerializeStringContent(Serialize(obj));

    /// <summary>
    /// Serialize object to string content using the default options
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <returns>The string content</returns>
    public static StringContent SerializeStringContent(object obj) =>
        SerializeStringContent(Serialize(obj));

    /// <summary>
    /// Serialize object to string content using the default options
    /// </summary>
    /// <param name="json">The JSON text</param>
    /// <returns>The string content</returns>
    public static StringContent SerializeStringContent(string json) =>
        new(json, Encoding.UTF8, "application/json");
}