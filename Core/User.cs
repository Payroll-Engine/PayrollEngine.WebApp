#if DEBUG
#define SUPERVISOR_ALL_FEATURES
#endif
using System;
using System.Collections.Generic;
using System.Linq;

namespace PayrollEngine.WebApp;

public class User : Client.Model.User
{
    public string FullName => $"{FirstName} {LastName}";

    // ReSharper disable once MemberCanBeProtected.Global
    public User()
    {
    }

    protected User(User copySource) :
        base(copySource)
    {
    }

    protected User(Client.Model.User copySource) :
        base(copySource)
    {
    }

    public bool HasPassword =>
        !string.IsNullOrWhiteSpace(Password);

    public bool Employee =>
        UserType == UserType.Employee;

    private bool Supervisor =>
        UserType == UserType.Supervisor;

    #region Tasks

    public int OpenTaskCount { get; set; }

    #endregion

    #region Feature

    public List<Feature> Features
    {
        get
        {
            var values = Attributes.GetMemberAttributeValue<string>()?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            return StringToFeatures(values);
        }
        set
        {
            EnsureAttributes();
            var values = FeaturesToCsv(value);
            Attributes.SetMemberAttributeValue(values);
        }
    }

    public string FeaturesAsString { get; set; } = string.Empty;
    public IEnumerable<string> FeaturesAsEnum
    {
        get => Features.Select(x => x.ToString());
        set => Features = StringToFeatures(value);
    }

    public bool HasAnyFeature()
    {
#if SUPERVISOR_ALL_FEATURES
        return Features.Any() || Supervisor;
#else
        return Features.Any();
#endif
    }

    /// <summary>
    /// Test for available feature
    /// </summary>
    /// <remarks>Enable all features for new users (debug only)</remarks>
    /// <param name="feature">The feature to test</param>
    public bool HasFeature(Feature feature)
    {
        // available
        var available = Features.Contains(feature);

#if SUPERVISOR_ALL_FEATURES
        if (!available && Supervisor)
        {
            return true;
        }
#endif

        // user setup für the supervisor
        if (!available && feature == Feature.Users && Supervisor)
        {
            return true;
        }
        return available;
    }

    //public bool HasAnyFeature(params Feature[] features) =>
    //    features.Any(HasFeature);

    private void AddFeature(Feature feature)
    {
        var features = Features;
        if (!features.Contains(feature))
        {
            features.Add(feature);
            Features = features;
        }
    }

    public void AddFeatures(IEnumerable<Feature> features) =>
        features?.ToList().ForEach(AddFeature);

    public bool RemoveFeature(Feature feature)
    {
        if (!HasFeature(feature))
        {
            return false;
        }

        var features = Features;
        features.Remove(feature);
        Features = features;
        return true;
    }

    private void EnsureAttributes() =>
        Attributes ??= new();

    private static List<Feature> StringToFeatures(IEnumerable<string> values)
    {
        var features = new List<Feature>();
        if (values != null)
        {
            foreach (var value in values)
            {
                if (Enum.TryParse<Feature>(value, out var feature))
                {
                    features.Add(feature);
                }
            }
        }
        return features;
    }

    private static string FeaturesToCsv(IEnumerable<Feature> values)
    {
        string featuresCsv = null;
        if (values != null)
        {
            var features = values.Select(x => x.ToString()).ToList();
            featuresCsv = string.Join(',', features);
        }
        return featuresCsv;
    }

    #endregion

    #region Settings

    public string StartupPage
    {
        get => Attributes.GetMemberAttributeValue<string>();
        set
        {
            EnsureAttributes();
            Attributes.SetMemberAttributeValue(value);
        }
    }

    public string StartupPayroll
    {
        get => Attributes.GetMemberAttributeValue<string>();
        set
        {
            EnsureAttributes();
            Attributes.SetMemberAttributeValue(value);
        }
    }

    public string StartupEmployee
    {
        get => Attributes.GetMemberAttributeValue<string>();
        set
        {
            EnsureAttributes();
            Attributes.SetMemberAttributeValue(value);
        }
    }

    #endregion

    /// <inheritdoc/>
    public override string ToString() =>
        $"{Identifier} {base.ToString()}";
}