﻿using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class SharedLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    // object
    public string ObjectId => PropertyValue();
    public string ObjectStatus => PropertyValue();
    public string ObjectCreated => PropertyValue();
    public string ObjectUpdated => PropertyValue();
    public string Immutable => PropertyValue();

    public string All => PropertyValue();
    public string SelectAll => PropertyValue();
    public string None => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string Hidden => PropertyValue();
    public string Owner => PropertyValue();
    public string Order => PropertyValue();
    public string Tags => PropertyValue();
    public string CommaSeparatedList => PropertyValue();
    public string Mandatory => PropertyValue();
    public string OverrideType => PropertyValue();
    public string Clusters => PropertyValue();

    public new string Key => PropertyValue();
    public string Field => PropertyValue();
    public string Start => PropertyValue();
    public string End => PropertyValue();
    public string Value => PropertyValue();
    public string NumericValue => PropertyValue();
    public string Text => PropertyValue();
    public string Type => PropertyValue();
    public string ValueType => PropertyValue();
    public string Search => PropertyValue();

    // values
    public string TrueToggle => PropertyValue();
    public string FalseToggle => PropertyValue();
    public string BooleanToggle(bool value) =>
        value ? TrueToggle : FalseToggle;


    // common
    public string Identifier => PropertyValue();
    public string Name => PropertyValue();
    public string Description => PropertyValue();
    public string Inheritance => PropertyValue();
    public string Culture => PropertyValue();
    public string Calendar => PropertyValue();

    public string FeaturesAdmin => PropertyValue();
    public string FeaturesSystem => PropertyValue();
    public string CommonFields => PropertyValue();

    // data
    public string FilterReset => PropertyValue();
    public string ExcelDownload => PropertyValue();
    public string DownloadCompleted => PropertyValue();

    public string ImmediateChanges => PropertyValue();
    public string JsonReformat => PropertyValue();
}