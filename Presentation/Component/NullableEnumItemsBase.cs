using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.Component;

public abstract class NullableEnumItemsBase<T> : ComponentBase
{
    [Parameter] public bool Ordered { get; set; } = true;
    [Inject] private Localizer Localizer { get; set; }

    protected List<LabeledValue<T>> Values { get; private set; } = [];

    private void SetupItems()
    {
        // nullable
        var type = typeof(T);
        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
        {
            type = nullableType;
        }

        // enum values
        var enumItems = Enum.GetValues(type).Cast<T>().ToList();

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

    protected override async Task OnInitializedAsync()
    {
        SetupItems();
        await base.OnInitializedAsync();
    }
}