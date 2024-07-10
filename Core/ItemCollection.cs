﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace PayrollEngine.WebApp;

/// <summary>
/// Implementation of a dynamic data collection based on generic Collection&lt;T&gt;,
/// implementing INotifyCollectionChanged to notify listeners
/// when items get added, removed or the whole list is refreshed.
/// </summary>
/// <remarks>source https://gist.github.com/weitzhandler/65ac9113e31d12e697cb58cd92601091</remarks>
public class ItemCollection<T> : ObservableCollection<T>, IDisposable
{
    [NonSerialized]
    private DeferredEventsCollection deferredEvents;

    #region Constructors

    /// <summary>
    /// Initializes a new instance of ObservableCollection that is empty and has default initial capacity.
    /// </summary>
    public ItemCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ObservableCollection class that contains
    /// elements copied from the specified collection and has sufficient capacity
    /// to accommodate the number of elements copied.
    /// </summary>
    /// <param name="items">The collection whose elements are copied to the new list.</param>
    /// <remarks>
    /// The elements are copied onto the ObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
    public ItemCollection(IEnumerable<T> items) :
        base(items)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ObservableCollection class
    /// that contains elements copied from the specified list
    /// </summary>
    /// <param name="items">The list whose elements are copied to the new list.</param>
    /// <remarks>
    /// The elements are copied onto the ObservableCollection in the
    /// same order they are read by the enumerator of the list.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> list is a null reference </exception>
    public ItemCollection(List<T> items) :
        base(items)
    {
    }

    #endregion Constructors

    #region Public Properties

    private EqualityComparer<T> comparer;
    private EqualityComparer<T> Comparer =>
        comparer ??= EqualityComparer<T>.Default;

    //private set => comparer = value;
    /// <summary>
    /// Gets or sets a value indicating whether this collection acts as a <see cref="HashSet{T}"/>,
    /// disallowing duplicate items, based on <see cref="Comparer"/>.
    /// This might indeed consume background performance, but in the other hand,
    /// it will pay off in UI performance as less required UI updates are required.
    /// </summary>
    private bool AllowDuplicates { get; } = true;

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
    /// </summary>
    /// <param name="items">
    /// The collection whose elements should be added to the end of the <see cref="ObservableCollection{T}"/>.
    /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        InsertRange(Count, items);
    }

    /// <summary>
    /// Inserts the elements of a collection into the <see cref="ObservableCollection{T}"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
    /// <param name="items">The collection whose elements should be inserted into the List{T}.
    /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>                
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not in the collection range.</exception>
    private void InsertRange(int index, IEnumerable<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (index > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        // remove duplicates
        if (!AllowDuplicates)
        {
            items = items.Distinct(Comparer)
                .Where(item => !Items.Contains(item, Comparer))
                .ToList();
        }

        // test empty collection
        var enumerable = items.ToList();
        if (items is ICollection<T> countable)
        {
            if (countable.Count == 0)
            {
                return;
            }
        }
        else if (!enumerable.Any())
        {
            return;
        }

        CheckReentrancy();

        //expand the following couple of lines when adding more constructors.
        var target = (List<T>)Items;
        target.InsertRange(index, enumerable);

        OnEssentialPropertiesChanged();

        if (!(items is IList list))
        {
            list = new List<T>(enumerable);
        }

        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, list, index));
    }

    /// <summary> 
    /// Removes the first occurrence of each item in the specified collection from the <see cref="ObservableCollection{T}"/>.
    /// </summary>
    /// <param name="items">The items to remove.</param>        
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
    public void RemoveRange(IEnumerable<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (Count == 0)
        {
            return;
        }

        var enumerable = items.ToList();
        // test collection
        if (items is ICollection<T> countable)
        {
            // empty collection
            if (countable.Count == 0)
            {
                return;
            }

            // single item collection
            if (countable.Count == 1)
            {
                using var enumerator = countable.GetEnumerator();
                enumerator.MoveNext();
                Remove(enumerator.Current);
                return;
            }
        }
        else if (!enumerable.Any())
        {
            return;
        }

        CheckReentrancy();

        // remove items and update clusters
        var clusters = new Dictionary<int, List<T>>();
        var lastIndex = -1;
        List<T> lastCluster = null;
        foreach (T item in enumerable)
        {
            var index = IndexOf(item);
            if (index < 0)
            {
                continue;
            }

            Items.RemoveAt(index);

            if (lastIndex == index && lastCluster != null)
            {
                lastCluster.Add(item);
            }
            else
            {
                clusters[lastIndex = index] = lastCluster = [item];
            }
        }

        OnEssentialPropertiesChanged();

        // notifications
        if (Count == 0)
        {
            OnCollectionReset();
        }
        else
        {
            foreach (var cluster in clusters)
            {
                OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, cluster.Value, cluster.Key));
            }
        }
    }

    /// <summary>
    /// Iterates over the collection and removes all items that satisfy the specified match.
    /// </summary>
    /// <remarks>The complexity is O(n).</remarks>
    /// <param name="match"></param>
    /// <returns>Returns the number of elements that where </returns>
    /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
    public int RemoveAll(Predicate<T> match) =>
        RemoveAll(0, Count, match);

    /// <summary>
    /// Iterates over the specified range within the collection and removes all items that satisfy the specified match.
    /// </summary>
    /// <remarks>The complexity is O(n).</remarks>
    /// <param name="index">The index of where to start performing the search.</param>
    /// <param name="count">The number of items to iterate on.</param>
    /// <param name="match"></param>
    /// <returns>Returns the number of elements that where </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
    private int RemoveAll(int index, int count, Predicate<T> match)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (index + count > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (match == null)
        {
            throw new ArgumentNullException(nameof(match));
        }
        if (Count == 0)
        {
            return 0;
        }

        List<T> cluster = null;
        var clusterIndex = -1;
        var removedCount = 0;

        using (BlockReentrancy())
        using (DeferEvents())
        {
            // remove items
            for (var i = 0; i < count; i++, index++)
            {
                T item = Items[index];
                if (match(item))
                {
                    Items.RemoveAt(index);
                    removedCount++;

                    // cluster update
                    if (clusterIndex == index)
                    {
                        Debug.Assert(cluster != null);
                        cluster!.Add(item);
                    }
                    else
                    {
                        cluster = [item];
                        clusterIndex = index;
                    }

                    index--;
                }
                else if (clusterIndex > -1)
                {
                    // cluster notification
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
                    clusterIndex = -1;
                    cluster = null;
                }
            }

            if (clusterIndex > -1)
            {
                // cluster notification
                OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
            }
        }

        if (removedCount > 0)
        {
            // properties notification
            OnEssentialPropertiesChanged();
        }

        return removedCount;
    }

    /// <summary>
    /// Removes a range of elements from the <see cref="ObservableCollection{T}"/>>.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
    /// <param name="count">The number of elements to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">The specified range is exceeding the collection.</exception>
    private void RemoveRange(int index, int count)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (index + count > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count == 0)
        {
            return;
        }

        if (count == 1)
        {
            RemoveItem(index);
            return;
        }

        //Items will always be List<T>, see constructors
        var items = (List<T>)Items;
        var removedItems = items.GetRange(index, count);

        CheckReentrancy();

        items.RemoveRange(index, count);

        OnEssentialPropertiesChanged();

        if (Count == 0)
        {
            OnCollectionReset();
        }
        else
        {
            OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, removedItems, index));
        }
    }

    /// <summary> 
    /// Clears the current collection and replaces it with the specified collection,
    /// using <see cref="Comparer"/>.
    /// </summary>             
    /// <param name="items">The items to fill the collection with, after clearing it.</param>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
    public void ReplaceRange(IEnumerable<T> items) =>
        ReplaceRange(0, Count, items);

    /// <summary>
    /// Removes the specified range and inserts the specified collection in its position, leaving equal items in equal positions intact.
    /// </summary>
    /// <param name="index">The index of where to start the replacement.</param>
    /// <param name="count">The number of items to be replaced.</param>
    /// <param name="insertItems">The collection to insert in that location.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="insertItems"/> is null.</exception>
    private void ReplaceRange(int index, int count, IEnumerable<T> insertItems)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (index + count > Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (insertItems == null)
        {
            throw new ArgumentNullException(nameof(insertItems));
        }

        // remove duplicates
        if (!AllowDuplicates)
        {
            insertItems = insertItems.Distinct(Comparer).ToList();
        }

        var enumerable = insertItems.ToList();
        // countable collection
        if (insertItems is ICollection<T> countable)
        {
            if (countable.Count == 0)
            {
                RemoveRange(index, count);
                return;
            }
        }
        // enumerable collection
        else if (!enumerable.Any())
        {
            RemoveRange(index, count);
            return;
        }

        // new item
        if (index + count == 0)
        {
            InsertRange(0, enumerable);
            return;
        }

        // list collection
        if (!(insertItems is IList<T> list))
        {
            list = new List<T>(enumerable);
        }

        using (BlockReentrancy())
        using (DeferEvents())
        {
            var rangeCount = index + count;
            var addedCount = list.Count;

            var changesMade = false;
            List<T> newCluster = null;
            List<T> oldCluster = null;

            var i = index;
            for (; i < rangeCount && i - index < addedCount; i++)
            {
                // parallel position
                T old = this[i], @new = list[i - index];
                if (Comparer.Equals(old, @new))
                {
                    OnRangeReplaced(i, newCluster!, oldCluster!);
                    continue;
                }

                Items[i] = @new;

                // cluster update
                if (newCluster == null)
                {
                    Debug.Assert(oldCluster == null);
                    newCluster = [@new];
                    oldCluster = [old];
                }
                else
                {
                    newCluster.Add(@new);
                    oldCluster!.Add(old);
                }

                changesMade = true;
            }

            OnRangeReplaced(i, newCluster!, oldCluster!);

            // exceeding position
            if (count != addedCount)
            {
                var items = (List<T>)Items;
                if (count > addedCount)
                {
                    var removedCount = rangeCount - addedCount;
                    var removed = new T[removedCount];
                    items.CopyTo(i, removed, 0, removed.Length);
                    items.RemoveRange(i, removedCount);
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, removed, i));
                }
                else
                {
                    var k = i - index;
                    var added = new T[addedCount - k];
                    for (var j = k; j < addedCount; j++)
                    {
                        T @new = list[j];
                        added[j - k] = @new;
                    }
                    items.InsertRange(i, added);
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Add, added, i));
                }

                OnEssentialPropertiesChanged();
            }
            else if (changesMade)
            {
                OnIndexerPropertyChanged();
            }
        }
    }

    #endregion Public Methods

    #region Protected Methods

    /// <summary>
    /// Called by base class Collection&lt;T&gt; when the list is being cleared;
    /// raises a CollectionChanged event to any listeners.
    /// </summary>
    protected override void ClearItems()
    {
        if (Count == 0)
        {
            return;
        }

        CheckReentrancy();
        base.ClearItems();
        OnEssentialPropertiesChanged();
        OnCollectionReset();
    }

    /// <inheritdoc/>
    protected override void InsertItem(int index, T item)
    {
        if (!AllowDuplicates && Items.Contains(item))
        {
            return;
        }

        base.InsertItem(index, item);
    }

    /// <inheritdoc/>
    protected override void SetItem(int index, T item)
    {
        if (AllowDuplicates)
        {
            if (Comparer.Equals(this[index], item))
            {
                return;
            }
        }
        else
        {
            if (Items.Contains(item, Comparer))
            {
                return;
            }
        }

        CheckReentrancy();
        T oldItem = this[index];
        base.SetItem(index, item);

        OnIndexerPropertyChanged();
        OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem!, item!, index);
    }

    // ReSharper disable CommentTypo
    /// <summary>
    /// Raise CollectionChanged event to any listeners.
    /// Properties/methods modifying this ObservableCollection will raise
    /// a collection changed event through this virtual method.
    /// </summary>
    /// <remarks>
    /// When overriding this method, either call its base implementation
    /// or call BlockReentrancy to guard against reentrant collection changes.
    /// </remarks>
    // ReSharper restore CommentTypo
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        if (deferredEvents != null)
        {
            deferredEvents.Add(args);
            return;
        }
        base.OnCollectionChanged(args);
    }

    private IDisposable DeferEvents() => new DeferredEventsCollection(this);

    #endregion Protected Methods

    #region Private Methods

    /// <summary>
    /// Helper to raise Count property and the Indexer property.
    /// </summary>
    private void OnEssentialPropertiesChanged()
    {
        OnPropertyChanged(ItemCollectionCache.CountPropertyChanged);
        OnIndexerPropertyChanged();
    }

    /// <summary>
    /// /// Helper to raise a PropertyChanged event for the Indexer property
    /// /// </summary>
    private void OnIndexerPropertyChanged() =>
        OnPropertyChanged(ItemCollectionCache.IndexerPropertyChanged);

    /// <summary>
    /// Helper to raise CollectionChanged event to any listeners
    /// </summary>
    private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index) =>
        OnCollectionChanged(new(action, newItem, oldItem, index));

    /// <summary>
    /// Helper to raise CollectionChanged event with action == Reset to any listeners
    /// </summary>
    private void OnCollectionReset() =>
        OnCollectionChanged(ItemCollectionCache.ResetCollectionChanged);

    /// <summary>
    /// Helper to raise event for clustered action and clear cluster.
    /// </summary>
    /// <param name="followingItemIndex">The index of the item following the replacement block.</param>
    /// <param name="newCluster"></param>
    /// <param name="oldCluster"></param>
    //TODO should have really been a local method inside ReplaceRange(int index, int count, IEnumerable<T> collection, IEqualityComparer<T> comparer),
    //move when supported language version updated.
    private void OnRangeReplaced(int followingItemIndex, ICollection<T> newCluster, ICollection<T> oldCluster)
    {
        if (oldCluster == null || oldCluster.Count == 0)
        {
            Debug.Assert(newCluster == null || newCluster.Count == 0);
            return;
        }

        OnCollectionChanged(new(
                NotifyCollectionChangedAction.Replace,
                new List<T>(newCluster),
                new List<T>(oldCluster),
                followingItemIndex - oldCluster.Count));

        oldCluster.Clear();
        newCluster.Clear();
    }

    #endregion Private Methods

    #region Private Types

    private sealed class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
    {
        private readonly ItemCollection<T> items;
        internal DeferredEventsCollection(ItemCollection<T> items)
        {
            Debug.Assert(items != null);
            Debug.Assert(items.deferredEvents == null);
            this.items = items;
            this.items.deferredEvents = this;
        }

        public void Dispose()
        {
            items.deferredEvents = null;
            foreach (var args in this)
            {
                items.OnCollectionChanged(args);
            }
        }
    }

    #endregion Private Types

    public void Dispose()
    {
        deferredEvents?.Dispose();
    }
}