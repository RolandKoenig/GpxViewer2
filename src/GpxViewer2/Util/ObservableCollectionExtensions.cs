using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GpxViewer2.Util;

// Logic is based on
// https://brianlagunas.com/write-a-sortable-observablecollection-for-wpf/

public static class ObservableCollectionExtensions
{
    public static void SortObservableCollection<T, TKey>(
        this ObservableCollection<T> collection,
        Func<T, TKey> keySelector, 
        ListSortDirection direction)
    {
        switch (direction)
        {
            case System.ComponentModel.ListSortDirection.Ascending:
            {
                ApplySort(
                    collection,
                    collection.OrderBy(keySelector));
                break;
            }
            case System.ComponentModel.ListSortDirection.Descending:
            {
                ApplySort(
                    collection,
                    collection.OrderByDescending(keySelector));
                break;
            }
        }
    }

    public static void SortObservableCollection<T, TKey>(
        this ObservableCollection<T> collection,
        Func<T, TKey> keySelector, 
        IComparer<TKey> comparer)
    {
        ApplySort(
            collection,
            collection.OrderBy(keySelector, comparer));
    }

    private static void ApplySort<T>(
        this ObservableCollection<T> collection,
        IEnumerable<T> sortedItems)
    {
        var sortedItemsList = sortedItems.ToList();

        foreach (var item in sortedItemsList)
        {
            collection.Move(
                collection.IndexOf(item), 
                sortedItemsList.IndexOf(item));
        }
    }
}