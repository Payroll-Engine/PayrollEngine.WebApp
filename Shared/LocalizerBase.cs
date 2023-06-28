using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public abstract class LocalizerBase
{
    public IStringLocalizerFactory Factory { get; }
    public IStringLocalizer Localizer { get; }
    public string GroupName { get; }

    private const string ResourceName = "Localizations";

    protected LocalizerBase(IStringLocalizerFactory factory, string groupName = null)
    {
        Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        
        // localization group: <GroupName>.<LocalizationKey>
        if (string.IsNullOrWhiteSpace(groupName))
        {
            groupName = GetType().Name.RemoveFromEnd(nameof(Localizer));
        }
        GroupName = groupName;

        // string localizer
        var assemblyName = GetType().Assembly.FullName;
        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            throw new ArgumentException(nameof(assemblyName));
        }
        Localizer = Factory.Create(ResourceName, assemblyName);
    }

    /// <summary>
    /// Retrieve translation from group and key
    /// Translation example: MyGroup.MyKey
    /// </summary>
    /// <param name="group">The localization group</param>
    /// <param name="key">The localization key</param>
    public string FromKey(string group, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException(nameof(key));
        }
        return group == null ? Localizer[key] : Localizer[$"{group}.{key}"];
    }

    /// <summary>
    /// Retrieve translation from group key, group and key are identical
    /// Resource name: MyText.MyText
    /// </summary>
    /// <param name="groupAndKey">The group key</param>
    public string FromGroupKey(string groupAndKey) =>
        FromKey(groupAndKey, groupAndKey);

    /// <summary>
    /// Retrieve translation from current group and key
    /// Resource name: {InternalGroupName}.MyText
    /// </summary>
    /// <param name="key">The localization key</param>
    public string FromKey(string key) =>
        FromKey(GroupName, key);

    /// <summary>
    /// Retrieve translation from property key
    /// Resource name: MyProperty.MyText
    /// </summary>
    /// <param name="caller">The caller method name</param>
    public string FromCaller([CallerMemberName] string caller = null) =>
        FromKey(caller);

    /// <summary>
    /// Retrieve translation from enum value
    /// Resource name: Enum.{EnumTypeName}.MyEnumValue
    /// </summary>
    /// <param name="value">The enum value</param>
    public string FromEnum<T>(T value)
    {
        var type = typeof(T);

        // nullable enum
        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
        {
            type = nullableType;
        }

        var group = $"{nameof(Enum)}.{type.Name}";
        var translation = FromKey(group, $"{value}");
        // missing translation: return the enum value
        if (string.IsNullOrEmpty(translation) || translation.StartsWith(group))
        {
            return value.ToString().ToPascalSentence();
        }
        return translation;
    }
}