using System;
using System.Linq;
using System.Collections.Generic;

namespace PayrollEngine.WebApp;

/// <summary>
/// Application user
/// </summary>
public class User : Client.Model.User
{
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Default constructor
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public User()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    protected User(User copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Copy base class constructor
    /// </summary>
    protected User(Client.Model.User copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Test for password
    /// </summary>
    public bool HasPassword =>
        !string.IsNullOrWhiteSpace(Password);

    #region Tasks

    /// <summary>
    /// Open task count
    /// </summary>
    public int OpenTaskCount { get; set; }

    #endregion

    #region Feature

    /// <summary>
    /// User features
    /// </summary>
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
            Attributes ??= new();
            Attributes.SetMemberAttributeValue(values);
        }
    }

    /// <summary>
    /// User features as string
    /// </summary>
    public string FeaturesAsString { get; set; } = string.Empty;

    /// <summary>
    /// User features as enum
    /// </summary>
    public IEnumerable<string> FeaturesAsEnum
    {
        get => Features.Select(x => x.ToString());
        set => Features = StringToFeatures(value);
    }

    /// <summary>
    /// Test user has any feature
    /// </summary>
    public bool HasAnyFeature() =>
        UserType switch
        {
            UserType.Administrator =>
                // user admin feature
                true,
            UserType.Supervisor =>
                // all features
                true,
            _ =>
                // feature list
                Features.Any()
        };

    /// <summary>
    /// Test for available feature
    /// </summary>
    /// <remarks>Enable all features for new users (debug only)</remarks>
    /// <param name="feature">The feature to test</param>
    public bool HasFeature(Feature feature) =>
        UserType switch
        {
            UserType.Administrator => 
                // user admin feature
                feature == Feature.Users,
            UserType.Supervisor => 
                // all features
                true,
            _ =>
                // test in feature list
                Features.Contains(feature)
        };

    /// <summary>
    /// Add feature
    /// </summary>
    /// <param name="feature">Feature to add</param>
    private void AddFeature(Feature feature)
    {
        var features = Features;
        if (!features.Contains(feature))
        {
            features.Add(feature);
            Features = features;
        }
    }

    /// <summary>
    /// Add features
    /// </summary>
    /// <param name="features">Features to add</param>
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

    /// <summary>
    /// User startup page
    /// </summary>
    public string StartupPage
    {
        get => Attributes.GetMemberAttributeValue<string>();
        set
        {
            EnsureAttributes();
            Attributes.SetMemberAttributeValue(value);
        }
    }
    /// <summary>
    /// User startup payroll
    /// </summary>

    public string StartupPayroll
    {
        get => Attributes.GetMemberAttributeValue<string>();
        set
        {
            EnsureAttributes();
            Attributes.SetMemberAttributeValue(value);
        }
    }

    /// <summary>
    /// User startup employee
    /// </summary>
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