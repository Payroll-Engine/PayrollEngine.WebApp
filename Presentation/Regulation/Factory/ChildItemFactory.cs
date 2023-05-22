using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Factory;

public abstract class ChildItemFactory<TParent, TObject> : ItemFactoryBase<TObject>
    where TParent : class, IRegulationItem
    where TObject : class, IRegulationItem
{

    protected ChildItemFactory(Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations) :
        base(tenant, payroll, regulations)
    {
    }

    protected abstract Task<List<TParent>> QueryParentObjects(int tenantId, int regulationId);
    protected abstract Task<List<TObject>> QueryChildObjects(int tenantId, TParent parentObject);

    /// <summary>
    /// Apply regulation
    /// </summary>
    /// <remarks>Do not call the base class method</remarks>
    /// <param name="items">The item collection</param>
    protected override async Task ApplyRegulationsAsync(List<TObject> items)
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

        // query parent regulation objects
        var parentItems = new List<Tuple<Client.Model.Regulation, List<TParent>>>();
        foreach (var regulation in Regulations)
        {
            var parents = await QueryParentObjects(Tenant.Id, regulation.Id);
            parentItems.Add(new(regulation, parents));
        }

        // query child regulation objects
        var clientItems = new List<Tuple<Client.Model.Regulation, TParent, List<TObject>>>();
        foreach (var parentObject in parentItems)
        {
            foreach (var parent in parentObject.Item2)
            {
                var child = await QueryChildObjects(Tenant.Id, parent);
                foreach (var obj in child)
                {
                    obj.Parent = parent;
                }
                clientItems.Add(new(parentObject.Item1, parent, child));
            }
        }

        // regulation items
        foreach (var item in items)
        {
            // regulation
            Client.Model.Regulation regulation = null;
            foreach (var clientObject in clientItems)
            {
                // find original object by id
                var obj = clientObject.Item3.FirstOrDefault(x => x.Id == item.Id);
                if (obj != null)
                {
                    regulation = clientObject.Item1;
                    // apply base object values (replace collected object values)
                    CopyTool.CopyProperties(obj, item);
                    break;
                }
            }
            if (regulation == null)
            {
                throw new PayrollException($"Unknown regulation for item {item}");
            }

            // ignore base items on the root regulation
            if (regulation != Regulations.Last())
            {
                var currentItem = item;
                // next child regulation index
                var index = Regulations.IndexOf(regulation) + 1;
                while (index < Regulations.Count)
                {
                    var baseRegulation = Regulations[index];
                    var regulationItems = clientItems.Where(x => x.Item1.Id == baseRegulation.Id);
                    foreach (var regulationItem in regulationItems)
                    {
                        var baseItem = regulationItem.Item3.FirstOrDefault(x => string.Equals(x.InheritanceKey, item.InheritanceKey));
                        if (baseItem != null)
                        {
                            // base item
                            baseItem.RegulationId = baseRegulation.Id;
                            baseItem.RegulationName = baseRegulation.Name;
                            // current item
                            currentItem.BaseItem = baseItem;
                            currentItem = baseItem;
                        }
                    }
                    index++;
                }
            }

            // inheritance
            RegulationInheritanceType inheritanceType;
            if (item.BaseItem == null)
            {
                // new or derived
                inheritanceType = regulation == Regulations.First() ?
                    RegulationInheritanceType.New :
                    RegulationInheritanceType.Derived;
            }
            else
            {
                // override or derived
                inheritanceType = regulation == Regulations.First() ?
                    RegulationInheritanceType.Override :
                    RegulationInheritanceType.Derived;
            }
            SetRegulation(regulation, item, inheritanceType);
        }
    }
}