using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PayrollEngine.WebApp;

/// <remarks>
/// To be kept outside <see cref="ObservableCollection{T}"/>, since otherwise, a new instance will be created for each generic type used.
/// </remarks>
public static class ItemCollectionCache
{
    public static readonly PropertyChangedEventArgs CountPropertyChanged = new("Count");
    public static readonly PropertyChangedEventArgs IndexerPropertyChanged = new("Item[]");
    public static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new(NotifyCollectionChangedAction.Reset);
}