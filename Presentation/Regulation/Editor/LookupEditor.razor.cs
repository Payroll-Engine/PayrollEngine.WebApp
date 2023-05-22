﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
    public partial class LookupEditor
    {
        [Parameter]
        public RegulationEditContext EditContext { get; set; }
        [Parameter]
        public IRegulationItem Item { get; set; }
        [Parameter]
        public EventCallback<IRegulationItem> SaveItem { get; set; }
        [Parameter]
        public EventCallback<IRegulationItem> DeleteItem { get; set; }
        [Parameter]
        public EventCallback<IRegulationItem> OverrideItem { get; set; }

        protected List<RegulationField> Fields { get; } = new() {
            new(nameof(RegulationLookup.Name), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = "Lookup name",
                RequiredError = "Name is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationLookup.Description), typeof(TextBox)),
            new(nameof(RegulationLookup.OverrideType), typeof(EnumListBox<OverrideType>)),
            new(nameof(RegulationLookup.RangeSize), typeof(NumericTextBox<decimal?>))
            {
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationLookup.Attributes), null)
        };
    }
}
