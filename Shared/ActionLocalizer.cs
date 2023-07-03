﻿using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ActionLocalizer : LocalizerBase
{
    public ActionLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Action => PropertyValue();
    public string Actions => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string MoveUp => PropertyValue();
    public string MoveDown => PropertyValue();

    public string ActionExpression => PropertyValue();
    public string Parameters => PropertyValue();
    public string Issues => PropertyValue();
    public string Source => PropertyValue();
    public string Namespace => PropertyValue();
    public string Categories => PropertyValue();
    public string AppendAction => PropertyValue();
}