using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NotebookPageStore<T> : MonoBehaviour, IReadOnlyList<T>, ICollection<T> where T : NotebookPage
{
    protected readonly SortedList<int, T> pages = new();

    public T this[int index] => pages.Values[index];

    public int Count => pages.Count;

    public bool IsReadOnly => false;

    public void Add(T item) => pages.Add(item.PageNumber, item);

    public void Clear() => pages.Clear();

    public bool Contains(T item) => pages.ContainsKey(item.PageNumber);

    public void CopyTo(T[] array, int arrayIndex) => pages.Values.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => pages.Values.GetEnumerator();

    public bool Remove(T item) => pages.Remove(item.PageNumber);

    IEnumerator IEnumerable.GetEnumerator() => pages.Values.GetEnumerator();
}
