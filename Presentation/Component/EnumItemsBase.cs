﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.Component;

public abstract class EnumItemsBase<T> : ComponentBase where T : struct, Enum
{
    [Parameter] public bool Ordered { get; set; } = true;
    [Inject] private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    protected List<LabeledValue<T>> Values { get; private set; } = [];

    private void SetupItems()
    {
        // enum values
        var enumItems = Enum.GetValues(typeof(T)).Cast<T>().ToList();

        // order
        if (Ordered)
        {
            enumItems = enumItems.OrderBy(x => x.ToString()).ToList();
        }

        // localized enum values
        List<LabeledValue<T>> values = [];
        foreach (var item in enumItems)
        {
            var label = Localizer != null ? Localizer.Enum(item) : item.ToString().ToPascalSentence();
            values.Add(new() { Value = item, Label = label });
        }
        Values = values;
    }

    protected override void OnInitialized()
    {
        SetupItems();
        base.OnInitialized();
    }
}