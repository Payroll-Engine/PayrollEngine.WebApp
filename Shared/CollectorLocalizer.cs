﻿using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CollectorLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Collector => PropertyValue();
    public string Collectors => PropertyValue();

    public string CollectMode => PropertyValue();
    public string Negated => PropertyValue();
    public string Threshold => PropertyValue();
    public string StartExpression => PropertyValue();
    public string ApplyExpression => PropertyValue();
    public string EndExpression => PropertyValue();
    public string MinResult => PropertyValue();
    public string MaxResult => PropertyValue();
}