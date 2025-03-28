﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public abstract class ItemFactoryBase<TObject>(Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations)
    : IItemFactory<TObject>
    where TObject : class, IRegulationItem
{
    /// <summary>
    /// The tenant
    /// </summary>
    protected Client.Model.Tenant Tenant { get; } = tenant ?? throw new ArgumentNullException(nameof(tenant));

    /// <summary>
    /// The payroll
    /// </summary>
    protected Client.Model.Payroll Payroll { get; } = payroll ?? throw new ArgumentNullException(nameof(payroll));

    /// <summary>
    /// The regulations
    /// </summary>
    protected List<Client.Model.Regulation> Regulations { get; } = regulations ?? throw new ArgumentNullException(nameof(regulations));

    protected abstract Task<List<TObject>> QueryItemsAsync(Client.Model.Regulation regulation);
    public abstract Task<List<TObject>> QueryPayrollItemsAsync();
    public async Task<List<TObject>> LoadPayrollItemsAsync()
    {
        var items = await QueryPayrollItemsAsync();
        await ApplyRegulationsAsync(items);
        return items;
    }

    protected virtual async Task ApplyRegulationsAsync(List<TObject> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        // empty source
        if (!items.Any() || !Regulations.Any())
        {
            return;
        }

        // query regulation objects
        var regulationObjects = new List<Tuple<Client.Model.Regulation, List<TObject>>>();
        foreach (var regulation in Regulations)
        {
            var objects = await QueryItemsAsync(regulation);
            regulationObjects.Add(new(regulation, objects));
        }

        // regulation items
        foreach (var item in items)
        {
            // regulation
            Client.Model.Regulation regulation = null;
            foreach (var regulationObject in regulationObjects)
            {
                // find original object by id
                var obj = regulationObject.Item2.FirstOrDefault(x => x.Id == item.Id);
                if (obj != null)
                {
                    regulation = regulationObject.Item1;
                    // apply base object values (replace collected object values)
                    CopyTool.CopyProperties(obj, item);
                    break;
                }
            }
            if (regulation == null)
            {
                throw new PayrollException($"Unknown regulation for item {item}.");
            }

            // base objects
            if (regulation != Regulations.Last())
            {
                var currentItem = item;
                // next to the current regulation
                var index = Regulations.IndexOf(regulation) + 1;
                while (index < Regulations.Count)
                {
                    // base object with the same inheritance key
                    var baseObject = regulationObjects[index].Item2.FirstOrDefault(x =>
                        string.Equals(x.InheritanceKey, item.InheritanceKey));
                    if (baseObject != null)
                    {
                        var baseRegulation = regulationObjects[index].Item1;
                        baseObject.RegulationName = baseRegulation.Name;

                        currentItem.BaseItem = baseObject;
                        currentItem = baseObject;
                    }
                    index++;
                }
            }

            // inheritance
            RegulationInheritanceType inheritanceType;
            if (item.BaseItem == null)
            {
                // new or base
                inheritanceType = regulation == Regulations.First() ?
                    RegulationInheritanceType.New :
                    RegulationInheritanceType.Base;
            }
            else
            {
                // derived or base
                inheritanceType = regulation == Regulations.First() ?
                    RegulationInheritanceType.Derived :
                    RegulationInheritanceType.Base;
            }
            SetRegulation(regulation, item, inheritanceType);
        }
    }

    protected void SetRegulation(Client.Model.Regulation regulation, IRegulationItem item, RegulationInheritanceType inheritanceType)
    {
        item.RegulationName = regulation.Name;
        item.InheritanceType = inheritanceType;
    }

    protected bool AddCollectionObject(ICollection<TObject> list, TObject item)
    {
        if (list == null)
        {
            return false;
        }

        var existing = list.FirstOrDefault(x => x.Id == item.Id);
        if (existing == null)
        {
            // create
            list.Add(item);
        }
        else
        {
            item.ApplyInheritanceKeyTo(existing);
        }
        return true;
    }

    protected TObject DeleteCollectionObject(ICollection<TObject> list, TObject item)
    {
        if (list == null)
        {
            return item;
        }

        var existing = list.FirstOrDefault(x => x.Id == item.Id);
        if (existing == null)
        {
            return item;
        }

        // remove
        list.Remove(item);

        // base object
        if (existing.BaseItem is TObject baseObject)
        {
            list.Add(baseObject);
            return baseObject;
        }

        return item;
    }

    protected void SetCreatedData(IRegulationItem source, IRegulationItem target)
    {
        target.Id = source.Id;
        target.Created = source.Created;
        target.Updated = source.Updated;
    }

    protected void SetUpdatedData(IRegulationItem target)
    {
        target.Updated = Date.Now;
    }

}